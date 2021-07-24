﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
                File.Copy(pluginPath, destFile, true);
                BepInExAssemblyInfo info = await FileTool.ReadDllInfoAsync(destFile).ConfigureAwait(false);
                IList<PluginInfo> plugins = await ListPluginsAsync(gamePath).ConfigureAwait(false);
                foreach (PluginInfo p in plugins.Where(x => x.Path != destFile && x.Id == info.Id))
                {
                    File.Delete(p.Path);
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

        public static async Task<IList<PluginInfo>> ListPluginsAsync(string path)
        {
            List<PluginInfo> plugins = new();
            string plguinDir = Path.Combine(Path.Combine(path, "BepInEx", "plugins"));
            List<Task> tasks = new();
            if (Directory.Exists(plguinDir))
            {
                foreach (string plugin in Directory.EnumerateFiles(plguinDir, "*.dll", SearchOption.AllDirectories))
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
                        BepInExAssemblyInfo dllInfo = await FileTool.ReadDllInfoAsync(plugin).ConfigureAwait(false);
                        if (!string.IsNullOrEmpty(dllInfo?.Id))
                        {
                            pluginInfo.Id = dllInfo.Id;
                            pluginInfo.Name = dllInfo.Name;
                            pluginInfo.Version = dllInfo.Version;
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
