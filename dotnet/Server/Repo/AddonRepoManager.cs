using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
        private static readonly HttpClient s_client = new HttpClient();

        private static readonly IDeserializer s_yamlDeserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

        private static readonly ISerializer s_yamlSerializer = new SerializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

        private const string RepoDirName = ".bierepo";

        private const string ConfigFileName = "config.yaml";

        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        public static AddonRepoManager Instance { get; } = new AddonRepoManager();

        public string RootPath { get; } = GetRootPath();

        public string ConfigFilePath => Path.Combine(RootPath, ConfigFileName);

        public string PluginStoreRoot => Path.Combine(RootPath, "plugins");

        public AddonRepoConfig Config { get; private set; }

        private AddonRepoManager()
        {
            Config = LoadConfig();
            if (!Directory.Exists(PluginStoreRoot))
            {
                Directory.CreateDirectory(PluginStoreRoot);
            }
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
            BepInExAssemblyInfo pi = await FileTool.ReadDllInfoAsync(file).ConfigureAwait(false);
            if (pi == null)
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

            string destDir = Path.Combine(PluginStoreRoot, Path.GetFileNameWithoutExtension(file));
            string destFile = Path.Combine(destDir, $"{Path.GetFileNameWithoutExtension(file)}.{pi.Version}.dll");
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }
            Logger.Info($"Saving plugin {pi.Name} - {pi.Version}");
            File.Copy(file, destFile, overwrite: true);
            File.WriteAllText($"{destFile}.json", JsonConvert.SerializeObject(pi));
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

        public async Task<IList<PluginInfo>> LoadLocalPluginsAsync()
        {
            List<PluginInfo> plugins = new List<PluginInfo>();
            foreach (string file in Directory.EnumerateFiles(PluginStoreRoot, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    string jsonPath = $"{file}.json";
                    if (File.Exists(jsonPath))
                    {
                        BepInExAssemblyInfo meta = null;
                        try
                        {
                            JsonConvert.DeserializeObject<BepInExAssemblyInfo>(File.ReadAllText(jsonPath));
                        }
                        catch { }
                        if (string.IsNullOrEmpty(meta?.Id))
                        {
                            meta = await FileTool.ReadDllInfoAsync(file).ConfigureAwait(false);
                            if (!string.IsNullOrEmpty(meta?.Id))
                            {
                                File.WriteAllText(jsonPath, JsonConvert.SerializeObject(meta));
                            }
                        }
                        if (!string.IsNullOrEmpty(meta?.Id))
                        {
                            plugins.Add(new PluginInfo
                            {
                                Path = file,
                                Id = meta.Id,
                                Name = meta.Name,
                                Version = meta.Version,
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
            return plugins;
        }

        public async Task UpdateBucketAsync()
        {
            HashSet<string> visited = new HashSet<string>();
            foreach (AddonRepoBucketConfig b in Config.Buckets)
            {
                await UpdateBucketInnerAsync(b, visited);
            }
        }

        private async Task UpdateBucketInnerAsync(AddonRepoBucketConfig bucket, ISet<string> visited)
        {
            if (string.IsNullOrEmpty(bucket.Url) || visited.Contains(bucket.Url))
            {
                return;
            }
            if (!Uri.TryCreate(bucket.Url, UriKind.Absolute, out Uri uri))
            {
                return;
            }
            Logger.Info($"Checking bucket url {bucket.Url}");
            visited.Add(bucket.Url);
            try
            {
                byte[] bytes = await s_client.GetByteArrayAsync(uri).ConfigureAwait(false);
                string str = Encoding.UTF8.GetString(bytes);
                List<AddonRepoBucketConfig> innerBuckets = null;
                // json
                try
                {
                    innerBuckets = JsonConvert.DeserializeObject<List<AddonRepoBucketConfig>>(str);

                }
                catch { }
                // yaml
                if (innerBuckets == null)
                {
                    try
                    {
                        innerBuckets = s_yamlDeserializer.Deserialize<List<AddonRepoBucketConfig>>(str);
                    }
                    catch { }
                }
                if (innerBuckets != null)
                {
                    foreach (AddonRepoBucketConfig b in innerBuckets)
                    {
                        await UpdateBucketInnerAsync(b, visited).ConfigureAwait(false);
                    }
                    return;
                }

                // webpage
                if (bucket.Patterns?.Count > 0)
                {
                    try
                    {
                        HtmlDocument html = new HtmlDocument();
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
                                foreach (string url in hrefs.Where(url => !string.IsNullOrEmpty(url) && globs.Any(g => g.IsMatch(url))))
                                {
                                    bool isAbsolute = url.StartsWith("http://") || url.StartsWith("https://");
                                    string absUrl = isAbsolute ? url : new UriBuilder(bucket.Url)
                                    {
                                        Path = url
                                    }.ToString();
                                    await UpdateBucketInnerAsync(new AddonRepoBucketConfig { Url = absUrl }, visited).ConfigureAwait(false);
                                }
                            }
                            return;
                        }
                    }
                    catch { }
                }

                try
                {
                    using MemoryStream ms = new MemoryStream(bytes) { Position = 0 };
                    using ZipArchive zip = new ZipArchive(ms);
                    foreach (ZipArchiveEntry entry in zip.Entries)
                    {
                        if (entry.FullName.EndsWith(".dll"))
                        {
                            using TempDir tempDir = new TempDir();
                            string destPath = Path.Combine(tempDir.Dir, Path.GetFileName(entry.FullName));
                            using (FileStream fileStream = File.OpenWrite(destPath))
                            {
                                using Stream entryStream = entry.Open();
                                await entryStream.CopyToAsync(fileStream).ConfigureAwait(false);
                            }
                            await AddPluginAsync(destPath, overwrite: true);
                        }
                    }
                }
                catch { }
            }
            catch (Exception oe)
            {
                Logger.Error(oe);
            }
        }

        private AddonRepoConfig LoadConfig()
        {
            if (File.Exists(ConfigFilePath))
            {
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
}
