using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BepInEx.ModManager.Shared;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace BepInEx.ModManager.Server.Services
{
    public class ModManagerServiceImpl : ModManagerService.ModManagerServiceBase
    {
        public override async Task<ListSteamGamesResponse> ListSteamGames(ListSteamGamesRequest request, ServerCallContext context)
        {
            var response = new ListSteamGamesResponse();
            await foreach (var g in GetSteamGamesAsync())
            {
                response.Games.Add(g);
            }
            return response;
        }

        public override async Task<CommonServiceResponse> InstallBIE(InstallBIERequest request, ServerCallContext context)
        {
            var unityPlayerPath = Path.Combine(request.Path, Constants.UnityPlayer);
            var is64bit = await FileTool.Is64BitAsync(unityPlayerPath).ConfigureAwait(false);
            await InstallationUtils.InstallBIEAsync(request.Path, is64bit).ConfigureAwait(false);
            // Install
            return new CommonServiceResponse
            {
                Success = true,
            };
        }

        public override Task<CommonServiceResponse> UninstallBIE(UninstallBIERequest request, ServerCallContext context)
        {
            return base.UninstallBIE(request, context);
        }

        public static async IAsyncEnumerable<GameInfo> GetSteamGamesAsync()
        {
            var uninstallEntry = Registry.LocalMachine
                .OpenSubKey("SOFTWARE")?
                .OpenSubKey("Microsoft")?
                .OpenSubKey("Windows")?
                .OpenSubKey("CurrentVersion")?
                .OpenSubKey("Uninstall");
            if (uninstallEntry != null)
            {
                foreach (var subKey in uninstallEntry.GetSubKeyNames())
                {
                    if (subKey.StartsWith("Steam App"))
                    {
                        var gameInfo = await ReadGameInfoAsync(uninstallEntry, subKey);
                        if (gameInfo != null)
                        {
                            yield return gameInfo;
                        }
                    }
                }
            }
        }

        private static async Task<GameInfo> ReadGameInfoAsync(RegistryKey uninstallEntry, string subKey)
        {
            if (!subKey.StartsWith("Steam App"))
            {
                return null;
            }
            var entry = uninstallEntry.OpenSubKey(subKey);
            var path = entry.GetValue("InstallLocation") as string;
            if (!Directory.Exists(path))
            {
                // TODO: Fix this hack, encoding should not be hard coded.
                path = Encoding.UTF8.GetString(Encoding.GetEncoding("gbk").GetBytes(path));
            }
            var name = entry.GetValue("DisplayName") as string;
            if (!string.IsNullOrEmpty(name) && !Directory.Exists(path))
            {
                path = Path.Combine(Path.GetDirectoryName(path), name);
                if (!Directory.Exists(path))
                {
                    return null;
                }
            }
            if (string.IsNullOrEmpty(name))
            {
                name = Path.GetFileName(path);
            }
            var unityPlayerPath = Path.Combine(path, Constants.UnityPlayer);
            if (!File.Exists(unityPlayerPath))
            {
                return null;
            }
            var gameInfo = new GameInfo
            {
                Id = Regex.Match(subKey, @"\d+", RegexOptions.Compiled).Value,
                Name = name,
                Path = path,
                Is64Bit = await FileTool.Is64BitAsync(unityPlayerPath).ConfigureAwait(false),
                IsBIEInstalled = Directory.Exists(Path.Combine(path, "BepInEx")),
                IsBIEInitialized = File.Exists(Path.Combine(path, "BepInEx", "config", "BepInEx.cfg")),
            };
            if (gameInfo.IsBIEInstalled)
            {
                var plguinDir = Path.Combine(Path.Combine(path, "BepInEx", "plugins"));
                var tasks = new List<Task>();
                if (Directory.Exists(plguinDir))
                {
                    foreach (var plugin in Directory.EnumerateFiles(plguinDir, "*.dll", SearchOption.AllDirectories))
                    {
                        var pluginInfo = new PluginInfo
                        {
                            Id = Path.GetFileNameWithoutExtension(plugin),
                            Path = plugin,
                            Version = "N/A",
                        };
                        pluginInfo.Name = pluginInfo.Id;
                        gameInfo.Plugins.Add(pluginInfo);
                        // Looks like AppDomain is being deprecated.
                        // Use a different way to extract assembly info instead, maybe ILSpy.
                        tasks.Add(Task.Run(async () =>
                        {
                            var dllInfo = await FileTool.ReadDllInfoAsync(plugin).ConfigureAwait(false);
                            if (dllInfo.Type == BepInExAssemblyType.Plugin)
                            {
                                pluginInfo.Id = dllInfo.Id;
                                pluginInfo.Name = dllInfo.Name;
                                pluginInfo.Version = dllInfo.Version;
                            }
                        }));
                    }
                }
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }

            var patcherDir = Path.Combine(Path.Combine(path, "BepInEx", "patchers"));
            if (Directory.Exists(patcherDir))
            {
                foreach (var patcher in Directory.EnumerateFiles(patcherDir, "*.dll", SearchOption.AllDirectories))
                {
                    var patchInfo = new PatcherInfo
                    {
                        Name = Path.GetFileNameWithoutExtension(patcher),
                        Path = patcher,
                    };
                    gameInfo.Patchers.Add(patchInfo);
                }
            }

            return gameInfo;
        }
    }
}
