using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BepInEx.ModManager.Server.Services;
using BepInEx.ModManager.Shared;
using DotNet.Globbing;
using HtmlAgilityPack;
using Newtonsoft.Json;
using NLog;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BepInEx.ModManager.Server.Repo
{
    public class AddonRepoManager
    {
        private static readonly HttpClient s_client = new();

        public static readonly IDeserializer s_yamlDeserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

        public static readonly ISerializer s_yamlSerializer = new SerializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

        private const string RepoDirName = ".bierepo";

        private const string ConfigFileName = "config.yaml";

        // Mutex?
        private readonly SemaphoreSlim _lockUpdateBuckets = new(1, 1);

        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        public static AddonRepoManager Instance { get; } = new();

        public string RootPath { get; } = GetRootPath();

        public string ConfigFilePath => Path.Combine(RootPath, ConfigFileName);

        public string PluginStoreRoot => Path.Combine(RootPath, "plugins");

        public string UrlCacheDir => Path.Combine(RootPath, ".url");

        public IReadOnlyList<PluginInfo> RepoPluginSnapshot { get; private set; }

        public AddonRepoConfig Config { get; private set; }

        private AddonRepoManager()
        {
            Config = LoadConfig();
            if (!Directory.Exists(PluginStoreRoot))
            {
                Directory.CreateDirectory(PluginStoreRoot);
            }

            // Temp Logic to allow manual config edit
            _ = Task.Run(async () =>
            {
                FileInfo fi = new(ConfigFilePath);
                while (true)
                {
                    try
                    {
                        FileInfo newFi = new(ConfigFilePath);
                        if (newFi.LastWriteTimeUtc != fi.LastWriteTimeUtc)
                        {
                            Logger.Info("Config file changed, reloading");
                            fi = newFi;
                            Config = s_yamlDeserializer.Deserialize<AddonRepoConfig>(File.ReadAllText(ConfigFilePath));
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                    await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                }
            });
        }

        public void SaveConfig(AddonRepoConfig config)
        {
            string content = s_yamlSerializer.Serialize(config);
            content = $"{AddonRepoConfig.HelpText.Trim()}\r\n\r\n{content}";
            File.WriteAllText(ConfigFilePath, content);
            Config = config;
        }

        public async Task<BepInExAssemblyInfo> AddPluginAsync(string file, bool overwrite = true)
        {
            if (!File.Exists(file))
            {
                return null;
            }
            if (!BepInExAssemblyInfo.TryRead(file, out BepInExAssemblyInfo pi, shallow: false))
            {
                return null;
            }

            // Check whether the same plugin with the same version exists
            if (overwrite)
            {
            }
            else
            {
            }

            string destDir = Path.Combine(PluginStoreRoot, Path.GetFileNameWithoutExtension(file), pi.Version);
            string destFile = Path.Combine(destDir, Path.GetFileName(file));
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }
            Logger.Info($"Saving plugin {pi.Name} - {pi.Version}");
            File.Copy(file, destFile, overwrite: true);
            return pi;
        }

        public void DeletePlugin(string file)
        {
            if (!file.StartsWith(PluginStoreRoot, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            string metadata = $"{file}.json";
            if (File.Exists(metadata))
            {
                File.Delete(metadata);
            }
        }

        public async Task<IReadOnlyList<PluginInfo>> LoadLocalPluginsAsync()
        {
            List<PluginInfo> plugins = new();
            foreach (string file in Directory.EnumerateFiles(PluginStoreRoot, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    if (BepInExAssemblyInfo.TryRead(file, out BepInExAssemblyInfo meta, shallow: false))
                    {
                        plugins.Add(new()
                        {
                            Path = file,
                            Id = meta.Id,
                            Name = meta.Name,
                            Version = meta.Version,
                            Desc = meta.Description ?? string.Empty,
                            Miscs = string.Join(",", meta.GameProcessName) ?? string.Empty,
                        });
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
            RepoPluginSnapshot = plugins;
            return plugins;
        }

        public async Task UpdateBucketsAsync()
        {
            if (!await _lockUpdateBuckets.WaitAsync(0).ConfigureAwait(false))
            {
                return;
            }
            try
            {
                // TODO: Thread safty
                HashSet<string> visited = new HashSet<string>();
                List<Task> tasks = new() { 
                    UpdateBucketInnerAsync(new()
                    {
                        Url = "https://bie-mod-repo.vercel.app/common.json",
                    }, visited)                    
                };

                if (ModManagerServiceImpl.GameSnapshot?.Count > 0)
                {
                    foreach (string id in ModManagerServiceImpl
                        .GameSnapshot
                        .Where(_ => !string.IsNullOrEmpty(_.Id))
                        .Select(_ => _.Id)
                        .Distinct())
                    {
                        // Make this configurable
                        string url = $"https://bie-mod-repo.vercel.app/{id}.json";
                        tasks.Add(UpdateBucketInnerAsync(new()
                        {
                            Url = url,
                        }, visited));
                    }
                }

                foreach (AddonRepoBucketConfig b in Config.Buckets.Distinct(AddonRepoBucketConfig.EqualityComparer.Instance))
                {
                    tasks.Add(UpdateBucketInnerAsync(b, visited));
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            finally
            {
                _lockUpdateBuckets.Release();
            }
        }

        private async Task<bool> UpdateBucketInnerAsync(AddonRepoBucketConfig bucket, ISet<string> visited)
        {
            if (string.IsNullOrEmpty(bucket.Url) || visited.Contains(bucket.Url))
            {
                return false;
            }
            if (!Uri.TryCreate(bucket.Url, UriKind.Absolute, out Uri uri))
            {
                return false;
            }

            visited.Add(bucket.Url);
            await Task.Yield();

            Logger.Info($"Checking bucket url {bucket.Url}");
            await ClientNotification.WriteAsync(new()
            {
                Message = $"Checking updates: {bucket.Url}",
            }).ConfigureAwait(false);

            string urlKey = HashUtils.GetMD5String(bucket.Url);
            string urlCacheFilePath = Path.Combine(UrlCacheDir, urlKey);
            UrlChangeDetectionHeaders urlCache = null;
            bool isUrlCacheChanged = false;
            bool isUrlCacheFileExpired = false;
            if (File.Exists(urlCacheFilePath))
            {
                try
                {
                    isUrlCacheFileExpired = new FileInfo(urlCacheFilePath).LastWriteTimeUtc.AddHours(6) < DateTimeOffset.UtcNow.UtcDateTime;
                    urlCache = JsonConvert.DeserializeObject<UrlChangeDetectionHeaders>(File.ReadAllText(urlCacheFilePath));
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
            if (urlCache == null)
            {
                urlCache = new();
            }

            bool hasErrors = false;
            try
            {
                using HttpResponseMessage headResponse = await s_client.SendAsync(new()
                {
                    Method = HttpMethod.Head,
                    RequestUri = uri,
                }).ConfigureAwait(false);
                long? contentLength = headResponse.Content.Headers.ContentLength;
                bool smallEnough = contentLength > 0 && contentLength < 1024 * 2;
                string etag = null;
                if (headResponse.Headers.TryGetValues("ETag", out IEnumerable<string> etagHeaderValues))
                {
                    etag = etagHeaderValues.FirstOrDefault();
                }
                string lastModifiedStr = null;
                if (headResponse.Headers.TryGetValues("Last-Modified", out IEnumerable<string> lmHeaderValues))
                {
                    lastModifiedStr = lmHeaderValues.FirstOrDefault();
                }
                if (!string.IsNullOrEmpty(etag))
                {
                    if (etag == urlCache.ETag)
                    {
                        if (!smallEnough && !isUrlCacheFileExpired)
                        {
                            Logger.Info("ETag cache hit");
                            return true;
                        }
                    }
                    else
                    {
                        urlCache.ETag = etag;
                        isUrlCacheChanged = true;
                    }
                }
                if (!string.IsNullOrEmpty(lastModifiedStr) && DateTimeOffset.TryParse(lastModifiedStr, out DateTimeOffset lastModified))
                {
                    if (lastModified == urlCache.LastModified)
                    {
                        if (!smallEnough && !isUrlCacheChanged && !isUrlCacheFileExpired)
                        {
                            Logger.Info("Last-Modified cache hit");
                            return true;
                        }
                    }
                    else
                    {
                        urlCache.LastModified = lastModified;
                        isUrlCacheChanged = true;
                    }
                }

                using HttpResponseMessage response = await s_client.GetAsync(uri).ConfigureAwait(false);
                // Too large file can be malicious
                if (response.Content.Headers.ContentLength > 5 * 1024 * 1024)
                {
                    return false;
                }
                byte[] bytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                string str = Encoding.UTF8.GetString(bytes);
                List<AddonRepoBucketConfig> innerBuckets = null;
                // json
                try
                {
                    innerBuckets = JsonConvert.DeserializeObject<List<AddonRepoBucketConfig>>(str);
                }
                catch { }
                // yaml
                if (innerBuckets?.Count == 0)
                {
                    try
                    {
                        innerBuckets = s_yamlDeserializer.Deserialize<List<AddonRepoBucketConfig>>(str);
                    }
                    catch { }
                }
                if (innerBuckets?.Count > 0)
                {
                    // Make urls absolute
                    foreach (AddonRepoBucketConfig ib in innerBuckets)
                    {
                        ib.Url = UrlUtils.GetAbsolutionUrl(url: ib.Url, referer: bucket.Url);
                    }

                    Task[] tasks = innerBuckets.Distinct(AddonRepoBucketConfig.EqualityComparer.Instance).Select(b => UpdateBucketInnerAsync(b, visited)).ToArray();
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                    return true;
                }

                // webpage
                if (bucket.Patterns?.Count > 0)
                {
                    try
                    {
                        HtmlDocument html = new();
                        html.LoadHtml(str);
                        HtmlNodeCollection anchors = html.DocumentNode.SelectNodes("//a");
                        if (anchors?.Count > 0)
                        {
                            List<string> hrefs = anchors.Select(a => a.GetAttributeValue("href", string.Empty)).ToList();
                            List<Glob> globs = new List<Glob>(bucket.Patterns.Count);
                            foreach (string p in bucket.Patterns)
                            {
                                try
                                {
                                    globs.Add(Glob.Parse(p));
                                }
                                catch { }
                            }
                            if (globs.Count > 0)
                            {

                                List<AddonRepoBucketConfig> innerBucketConfigs = new();
                                foreach (string url in hrefs.Where(url => !string.IsNullOrEmpty(url) && globs.Any(g => g.IsMatch(url))))
                                {
                                    bool isAbsolute = url.StartsWith("http://") || url.StartsWith("https://");
                                    string absUrl = isAbsolute ? url : new UriBuilder(bucket.Url)
                                    {
                                        Path = url
                                    }.ToString();
                                    innerBucketConfigs.Add(new() { Url = UrlUtils.GetAbsolutionUrl(url: url, referer: bucket.Url) });
                                }
                                Task[] tasks = innerBucketConfigs.Distinct(AddonRepoBucketConfig.EqualityComparer.Instance).Select(b => UpdateBucketInnerAsync(b, visited)).ToArray();
                                await Task.WhenAll(tasks).ConfigureAwait(false);
                            }
                            return true;
                        }
                    }
                    catch { }
                }

                if (await TryAddPlugArchive(bytes).ConfigureAwait(false))
                {
                    return true;
                }
            }
            catch (Exception oe)
            {
                hasErrors = true;
                Logger.Error(oe);
            }
            finally
            {
                if (!hasErrors && isUrlCacheChanged && urlCache.IsValid())
                {
                    if (!Directory.Exists(UrlCacheDir))
                    {
                        Directory.CreateDirectory(UrlCacheDir);
                    }
                    File.WriteAllText(urlCacheFilePath, JsonConvert.SerializeObject(urlCache));
                }
            }
            return true;
        }

        public async Task<bool> TryAddPlugArchive(byte[] bytes)
        {
            try
            {
                using MemoryStream ms = new(bytes) { Position = 0 };
                using ZipArchive zip = new(ms);
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    if (entry.FullName.EndsWith(".dll"))
                    {
                        await TryAddDllAsync(entry.FullName, entry.Open()).ConfigureAwait(false);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return false;
            }
        }

        public async Task<bool> TryAddDllAsync(string fileName, Stream stream)
        {
            try
            {
                using TempDir tempDir = new();
                string destPath = Path.Combine(tempDir.Dir, Path.GetFileName(fileName));
                using (FileStream fileStream = File.OpenWrite(destPath))
                {
                    await stream.CopyToAsync(fileStream).ConfigureAwait(false);
                }
                BepInExAssemblyInfo r = await AddPluginAsync(destPath, overwrite: true).ConfigureAwait(false);
                return r != null;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return false;
        }

        private AddonRepoConfig LoadConfig()
        {
            if (File.Exists(ConfigFilePath))
            {
                Logger.Info($"Loading plugin repo config {ConfigFilePath}");
                try
                {
                    return s_yamlDeserializer.Deserialize<AddonRepoConfig>(File.ReadAllText(ConfigFilePath));
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
            if (!Directory.Exists(RootPath))
            {
                Directory.CreateDirectory(RootPath);
            }

            AddonRepoConfig config = AddonRepoConfig.Default;
            SaveConfig(config);
            return config;
        }

        private static string GetRootPath()
        {
            string root = Environment.GetEnvironmentVariable("BIE_REPO_ROOT");
            if (string.IsNullOrEmpty(root))
            {
                string parrentDir = Environment.GetEnvironmentVariable("USERPROFILE");
                if (string.IsNullOrEmpty(parrentDir))
                {
                    parrentDir = @"c:\";
                }

                root = Path.Combine(parrentDir, RepoDirName);
            }

            if (!Directory.Exists(root))
            {
                try
                {
                    Directory.CreateDirectory(root);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }

            return root;
        }
    }

    public class UrlChangeDetectionHeaders
    {
        public DateTimeOffset LastModified { get; set; }

        public string ETag { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(ETag) || LastModified.AddYears(100) > DateTimeOffset.Now;
        }
    }
}
