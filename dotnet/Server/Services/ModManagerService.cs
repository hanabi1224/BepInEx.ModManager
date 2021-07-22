using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            foreach (var g in GetSteamGames())
            {
                response.Games.Add(g);
            }
            return response;
        }

        public static IEnumerable<GameInfo> GetSteamGames()
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
                                continue;
                            }
                        }
                        if (string.IsNullOrEmpty(name))
                        {
                            name = Path.GetFileName(path);
                        }
                        var gameInfo = new GameInfo
                        {
                            Id = Regex.Match(subKey, @"\d+").Value,
                            Name = name,
                            Path = path,
                            IsBIEInstalled = Directory.Exists(Path.Combine(path, "BepInEx")),
                            IsBIEInitialized = File.Exists(Path.Combine(path, "BepInEx", "config", "BepInEx.cfg")),
                        };
                        if (gameInfo.IsBIEInstalled)
                        {
                            var plguinDir = Path.Combine(Path.Combine(path, "BepInEx", "plugins"));
                            if (Directory.Exists(plguinDir))
                            {
                                foreach (var plugin in Directory.EnumerateFiles(plguinDir, "*.dll", SearchOption.AllDirectories))
                                {
                                    var pluginInfo = new PluginInfo
                                    {
                                        Name = Path.GetFileNameWithoutExtension(plugin),
                                        Path = plugin,
                                    };
                                    gameInfo.Plugins.Add(pluginInfo);
                                    // Looks like AppDomain is being deprecated.
                                    // Use a different way to extract assembly info instead, maybe ILSpy.
                                    //var appDomain = AppDomain.CreateDomain(plugin);
                                    //try
                                    //{
                                    //    appDomain.Load(plugin);
                                    //    foreach (var asm in appDomain.GetAssemblies())
                                    //    {
                                    //        foreach (var t in asm.GetTypes())
                                    //        {
                                    //            foreach (var attr in t.GetCustomAttributes(false))
                                    //            {
                                    //                if (attr is BepInPlugin pluginAttr)
                                    //                {
                                    //                    var pi = new PluginInfo
                                    //                    {
                                    //                        Path = plugin,
                                    //                    };
                                    //                    gi.Plugins.Add(pi);
                                    //                    pi.Id = pluginAttr.GUID;
                                    //                    pi.Name = pluginAttr.Name;
                                    //                    pi.Version = pluginAttr.Version.ToString();
                                    //                }
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                    //catch (Exception e)
                                    //{
                                    //    Console.Error.WriteLine(e);
                                    //}
                                    //finally
                                    //{
                                    //    AppDomain.Unload(appDomain);
                                    //}
                                }
                            }
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

                        yield return gameInfo;
                    }
                }
            }
        }
    }
}
