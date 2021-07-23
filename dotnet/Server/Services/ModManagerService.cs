using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BepInEx.ModManager.Shared;
using Grpc.Core;
using Microsoft.Win32;

namespace BepInEx.ModManager.Server.Services
{
    public class ModManagerServiceImpl : ModManagerService.ModManagerServiceBase
    {
        public override async Task<ListSteamGamesResponse> ListSteamGames(ListSteamGamesRequest request, ServerCallContext context)
        {
            ListSteamGamesResponse response = new ListSteamGamesResponse();
            await foreach (GameInfo g in GetSteamGamesAsync())
            {
                response.Games.Add(g);
            }
            return response;
        }

        public override async Task<CommonServiceResponse> InstallBIE(InstallBIERequest request, ServerCallContext context)
        {
            string unityPlayerPath = Path.Combine(request.Path, Constants.UnityPlayer);
            bool is64bit = await FileTool.Is64BitAsync(unityPlayerPath).ConfigureAwait(false);
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
            RegistryKey uninstallEntry = Registry.LocalMachine
                .OpenSubKey("SOFTWARE")?
                .OpenSubKey("Microsoft")?
                .OpenSubKey("Windows")?
                .OpenSubKey("CurrentVersion")?
                .OpenSubKey("Uninstall");
            if (uninstallEntry != null)
            {
                foreach (string subKey in uninstallEntry.GetSubKeyNames())
                {
                    if (subKey.StartsWith("Steam App"))
                    {
                        GameInfo gameInfo = await ReadGameInfoAsync(uninstallEntry, subKey);
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
            RegistryKey entry = uninstallEntry.OpenSubKey(subKey);
            string path = entry.GetValue("InstallLocation") as string;
            if (!Directory.Exists(path))
            {
                // TODO: Fix this hack, encoding should not be hard coded.
                path = Encoding.UTF8.GetString(Encoding.GetEncoding("gbk").GetBytes(path));
            }
            string name = entry.GetValue("DisplayName") as string;
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
            string unityPlayerPath = Path.Combine(path, Constants.UnityPlayer);
            if (!File.Exists(unityPlayerPath))
            {
                return null;
            }
            GameInfo gameInfo = new GameInfo
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
                string plguinDir = Path.Combine(Path.Combine(path, "BepInEx", "plugins"));
                List<Task> tasks = new List<Task>();
                if (Directory.Exists(plguinDir))
                {
                    foreach (string plugin in Directory.EnumerateFiles(plguinDir, "*.dll", SearchOption.AllDirectories))
                    {
                        PluginInfo pluginInfo = new PluginInfo
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
            }

            string patcherDir = Path.Combine(Path.Combine(path, "BepInEx", "patchers"));
            if (Directory.Exists(patcherDir))
            {
                foreach (string patcher in Directory.EnumerateFiles(patcherDir, "*.dll", SearchOption.AllDirectories))
                {
                    PatcherInfo patchInfo = new PatcherInfo
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
