using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BepInEx.ModManager.Server.Repo;
using BepInEx.ModManager.Shared;
using HtmlAgilityPack;
using NLog;

namespace BepInEx.ModManager.Server
{
    public static class InstallationUtils
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        private static readonly HttpClient s_client = new();
        public static async Task<string> GetLatestBIEDownloadUrlAsync(bool is64bit)
        {
            HtmlDocument html = new();
            html.Load(await s_client.GetStreamAsync(Constants.BepInExLatestReleasePage).ConfigureAwait(false));
            HtmlNodeCollection anchors = html.DocumentNode.SelectNodes("//a");
            string toContain = is64bit ? "BepInEx_x64" : "BepInEx_x86";
            string href = anchors.Select(a => a.GetAttributeValue("href", string.Empty)).FirstOrDefault(h => h.Contains(".zip", StringComparison.OrdinalIgnoreCase) && h.Contains(toContain, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(href) && href.StartsWith("/"))
            {
                UriBuilder ub = new(Constants.BepInExLatestReleasePage)
                {
                    Path = href
                };
                href = ub.ToString();
            }

            return href;
        }

        public static async Task InstallRemoteBIEAsync(string path, bool is64bit)
        {
            string zipUrl = await GetLatestBIEDownloadUrlAsync(is64bit).ConfigureAwait(false);
            Stream zipStream = await s_client.GetStreamAsync(zipUrl).ConfigureAwait(false);
            using ZipArchive zip = new(zipStream);
            foreach (ZipArchiveEntry entry in zip.Entries)
            {
                string targetPath = Path.Combine(path, entry.FullName);
                string targetPathDir = Path.GetDirectoryName(targetPath);
                if (!Directory.Exists(targetPathDir))
                {
                    Directory.CreateDirectory(targetPathDir);
                }
                using FileStream fileStream = File.OpenWrite(targetPath);
                using Stream entryStream = entry.Open();
                await entryStream.CopyToAsync(fileStream).ConfigureAwait(false);
            }
        }

        public static async Task InstallLocalBIEAsync(string path, bool is64bit)
        {
            string zipFile = Directory
                .EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "*.zip", SearchOption.AllDirectories)
                .FirstOrDefault(_ => _.Contains("BepInEx", StringComparison.OrdinalIgnoreCase) && _.Contains(is64bit ? "_x64_" : "_x86_", StringComparison.OrdinalIgnoreCase));
            using ZipArchive zip = new(File.OpenRead(zipFile));
            foreach (ZipArchiveEntry entry in zip.Entries)
            {
                string targetPath = Path.Combine(path, entry.FullName);
                string targetPathDir = Path.GetDirectoryName(targetPath);
                if (!Directory.Exists(targetPathDir))
                {
                    Directory.CreateDirectory(targetPathDir);
                }
                using FileStream fileStream = File.OpenWrite(targetPath);
                using Stream entryStream = entry.Open();
                await entryStream.CopyToAsync(fileStream).ConfigureAwait(false);
            }
        }

        public static async Task<bool> UninstallBIEAsync(string path)
        {
            if (!IsUnityGameRoot(path))
            {
                return false;
            }
            string bieDir = Path.Combine(path, "BepInEx");
            if (!Directory.Exists(bieDir))
            {
                return false;
            }

            Directory.Delete(bieDir, true);

            return true;
        }

        public static async Task<bool> IntallPluginAsync(string gamePath, string pluginPath)
        {
            if (IsUnityGameRoot(gamePath) && File.Exists(pluginPath))
            {
                Logger.Info($"Game path:{gamePath}\nPlugin path:{pluginPath}");
                string destDir = Path.Combine(gamePath, "BepInEx", "plugins");
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }
                string destFile = Path.Combine(destDir, Path.GetFileName(pluginPath));
                // Might be identical when installing missing dependencies
                if (pluginPath != destFile)
                {
                    File.Copy(pluginPath, destFile, true);
                    Logger.Info($"Plugin installed: {Path.GetFileName(pluginPath)}");
                }
                if (BepInExAssemblyInfo.TryRead(pluginPath, out BepInExAssemblyInfo info, shallow: false))
                {
                    IReadOnlyList<PluginInfo> plugins = await ListPluginsAsync(gamePath).ConfigureAwait(false);
                    foreach (PluginInfo p in plugins.Where(x => x.Path != destFile && x.Id == info.Id))
                    {
                        File.Delete(p.Path);
                    }
                    // References
                    if (info.References?.Count > 0)
                    {
                        foreach (BepInExAssemblyReferenceInfo reference in info.References)
                        {
                            // Match version first
                            string localRefFile = Path.Combine(destDir, AddonRepoManager.Instance.PluginStoreRoot, reference.Version, reference.Name, $"{reference.Name}.dll");
                            if (!File.Exists(localRefFile))
                            {
                                localRefFile = Directory
                                    .EnumerateFiles(AddonRepoManager.Instance.PluginStoreRoot, $"{reference.Name}.dll", SearchOption.AllDirectories)
                                    .OrderByDescending(_ => _) // Latest version
                                    .FirstOrDefault();
                            }
                            if (!string.IsNullOrEmpty(localRefFile) && File.Exists(localRefFile))
                            {
                                foreach (string dup in Directory.EnumerateFiles(destDir, $"{reference.Name}.dll", SearchOption.AllDirectories))
                                {
                                    try
                                    {
                                        File.Delete(dup);
                                        Logger.Info($"Deleting duplicate reference: {Path.GetFileName(dup)}");
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Error(e);
                                    }
                                }

                                string destRefFile = Path.Combine(destDir, $"{reference.Name}.dll");
                                File.Copy(localRefFile, destRefFile, true);
                                Logger.Info($"Dependency installed: {reference.Name}.dll");
                            }
                        }
                    }
                }
            }
            return true;
        }

        public static async Task<bool> UninstallPluginAsync(string pluginPath)
        {
            if (File.Exists(pluginPath))
            {
                string parent = Path.GetDirectoryName(pluginPath);
                for (int i = 0; i < 10; i++, parent = Path.GetDirectoryName(parent))
                {
                    if (IsUnityGameRoot(parent))
                    {
                        File.Delete(pluginPath);
                        return true;
                    }
                }
            }
            return false;
        }

        public static async Task<IReadOnlyList<PluginInfo>> ListPluginsAsync(string path)
        {
            List<PluginInfo> plugins = new();
            string bieCoreDir = Path.Combine(Path.Combine(path, "BepInEx", "core"));
            string pluginDir = Path.Combine(Path.Combine(path, "BepInEx", "plugins"));
            List<Task> tasks = new();
            if (Directory.Exists(pluginDir))
            {
                foreach (string plugin in Directory.EnumerateFiles(pluginDir, "*.dll", SearchOption.AllDirectories))
                {
                    PluginInfo pluginInfo = new()
                    {
                        Id = Path.GetFileNameWithoutExtension(plugin),
                        Path = plugin,
                        Version = "N/A",
                    };
                    pluginInfo.Name = pluginInfo.Id;
                    plugins.Add(pluginInfo);
                    // Looks like AppDomain is being deprecated.
                    // Use a different way to extract assembly info instead, maybe ILSpy.
                    tasks.Add(Task.Run(async () =>
                    {
                        if (BepInExAssemblyInfo.TryRead(plugin, out BepInExAssemblyInfo dllInfo, shallow: false))
                        {
                            pluginInfo.Id = dllInfo.Id;
                            pluginInfo.Name = dllInfo.Name;
                            pluginInfo.Version = dllInfo.Version;
                            pluginInfo.Desc = dllInfo.Description ?? string.Empty;
                            pluginInfo.Miscs = string.Join(",", dllInfo.GameProcessName) ?? string.Empty;

                            // Check missing dependencies
                            if (dllInfo.References?.Count > 0)
                            {
                                foreach (BepInExAssemblyReferenceInfo reference in dllInfo.References)
                                {
                                    string refFileName = $"{reference.Name}.dll";
                                    if (!Directory.EnumerateFiles(pluginDir, refFileName, SearchOption.AllDirectories).Any()
                                        && !Directory.EnumerateFiles(bieCoreDir, refFileName, SearchOption.AllDirectories).Any())
                                    {
                                        Logger.Info($"Missing reference {refFileName}");
                                        pluginInfo.MissingReference = true;
                                        break;
                                    }
                                }
                            }

                            // Check local updates
                            IReadOnlyList<PluginInfo> repoPlugins = AddonRepoManager.Instance.RepoPluginSnapshot;
                            if (repoPlugins?.Count > 0)
                            {
                                PluginInfo latest = repoPlugins
                                    .Where(_ => _.Id == dllInfo.Id && _.Version.CompareTo(dllInfo.Version) > 0)
                                    .OrderByDescending(_ => _.Version)
                                    .FirstOrDefault();
                                if (latest != null)
                                {
                                    pluginInfo.HasUpdate = true;
                                    pluginInfo.UpgradePath = latest.Path;
                                }
                            }
                        }
                    }));
                }
            }
            await Task.WhenAll(tasks).ConfigureAwait(false);
            return plugins;
        }

        public static bool IsUnityGameRoot(string dir)
        {
            return File.Exists(Path.Combine(dir, "UnityPlayer.dll"));
        }
    }
}
