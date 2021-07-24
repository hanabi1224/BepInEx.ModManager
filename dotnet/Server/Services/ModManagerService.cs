using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using BepInEx.ModManager.Server.Repo;
using Grpc.Core;
using Microsoft.Win32;
using NLog;

namespace BepInEx.ModManager.Server.Services
{
    public class ModManagerServiceImpl : ModManagerService.ModManagerServiceBase
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        public override async Task LongConnect(
            LongConnectRequest request,
            IServerStreamWriter<LongConnectResponse> responseStream,
            ServerCallContext context)
        {
            CancellationToken token = context.CancellationToken;
            Channel<LongConnectResponse> channel = Channel.CreateUnbounded<LongConnectResponse>();
            ClientNotification.ChannelWriter = channel.Writer;
            while (!token.IsCancellationRequested && ClientNotification.ChannelWriter == channel.Writer)
            {
                LongConnectResponse msg = await channel.Reader.ReadAsync(token).ConfigureAwait(false);
                await responseStream.WriteAsync(msg).ConfigureAwait(false);
                await Task.Delay(TimeSpan.FromSeconds(1), token).ConfigureAwait(false);
            }
            if (ClientNotification.ChannelWriter == channel.Writer)
            {
                ClientNotification.ChannelWriter = null;
            }
        }

        public override async Task<ListGamesResponse> ListGames(ListGamesRequest request, ServerCallContext context)
        {
            ListGamesResponse response = new();
            foreach (GameInfo g in await GetGamesAsync().ConfigureAwait(false))
            {
                response.Games.Add(g);
            }
            return response;
        }

        public override async Task<CommonServiceResponse> InstallBIE(InstallBIERequest request, ServerCallContext context)
        {
            string unityPlayerPath = Path.Combine(request.GamePath, Constants.UnityPlayer);
            bool is64bit = await FileTool.Is64BitAsync(unityPlayerPath).ConfigureAwait(false);
            await InstallationUtils.InstallLocalBIEAsync(request.GamePath, is64bit).ConfigureAwait(false);
            // Install
            return new()
            {
                Success = true,
            };
        }

        public override async Task<CommonServiceResponse> UninstallBIE(UninstallBIERequest request, ServerCallContext context)
        {
            bool success = await InstallationUtils.UninstallBIEAsync(request.GamePath).ConfigureAwait(false);
            return new() { Success = success };
        }

        public override async Task<ListPluginsResponse> ListPlugins(ListPluginsRequest request, ServerCallContext context)
        {
            ListPluginsResponse response = new();
            IList<PluginInfo> localPlugins = await AddonRepoManager.Instance.LoadLocalPluginsAsync().ConfigureAwait(false);
            foreach (PluginInfo p in localPlugins)
            {
                response.Plugins.Add(p);
            }
            return response;
        }

        public override async Task<CommonServiceResponse> AddPluginToRepo(AddPluginToRepoRequest request, ServerCallContext context)
        {
            byte[] bytes = request.Data.ToByteArray();
            bool success = await AddonRepoManager.Instance.TryAddPlugArchive(bytes).ConfigureAwait(false);
            if (!success)
            {
                using MemoryStream ms = new(bytes) { Position = 0 };
                success = await AddonRepoManager.Instance.TryAddDllAsync(request.FileName, ms).ConfigureAwait(false);
            }
            return new() { Success = success };
        }

        public override async Task<CommonServiceResponse> CheckPluginUpdates(CheckPluginUpdatesRequest request, ServerCallContext context)
        {
            await AddonRepoManager.Instance.UpdateBucketsAsync().ConfigureAwait(false);
            return new() { Success = true };
        }

        public override async Task<CommonServiceResponse> InstallPlugin(InstallPluginRequest request, ServerCallContext context)
        {
            bool success = await InstallationUtils.IntallPluginAsync(
                gamePath: request.GamePath,
                pluginPath: request.PluginPath).ConfigureAwait(false);

            return new() { Success = success };
        }

        public override async Task<CommonServiceResponse> UninstallPlugin(UninstallPluginRequest request, ServerCallContext context)
        {
            bool success = await InstallationUtils.UninstallPluginAsync(request.PluginPath).ConfigureAwait(false);
            return new()
            {
                Success = success
            };
        }

        public override async Task<ReadConfigResponse> ReadConfig(ReadConfigRequest request, ServerCallContext context)
        {
            return new()
            {
                Content = await File.ReadAllTextAsync(AddonRepoManager.Instance.ConfigFilePath).ConfigureAwait(false),
            };
        }

        public override async Task<CommonServiceResponse> WriteConfig(WriteConfigRequest request, ServerCallContext context)
        {
            string content = request.Content;
            try
            {
                AddonRepoConfig config = AddonRepoManager.s_yamlDeserializer.Deserialize<AddonRepoConfig>(content);
                AddonRepoManager.Instance.SaveConfig(config);
                return new()
                {
                    Success = true,
                };
            }
            catch
            {
                return new()
                {
                    Success = false,
                    Error = "File format is invalid",
                };
            }
        }

        public override async Task<CommonServiceResponse> AddGame(AddGameRequest request, ServerCallContext context)
        {
            if (InstallationUtils.IsUnityGameRoot(request.Path))
            {
                AddonRepoConfig config = AddonRepoManager.Instance.Config;
                config.Games.Add(new()
                {
                    Name = string.IsNullOrWhiteSpace(request.Name) ? Path.GetFileName(request.Path).Trim() : request.Name.Trim(),
                    Path = request.Path,
                });
                AddonRepoManager.Instance.SaveConfig(config);
                return new() { Success = true };
            }
            else
            {
                return new()
                {
                    Success = false,
                    Error = "Not a valid unity game",
                };
            }
        }

        public static async Task<IList<GameInfo>> GetGamesAsync()
        {
            List<GameInfo> games = new();
            List<Task<GameInfo>> tasks = new();
            if (AddonRepoManager.Instance.Config.Games.Count > 0)
            {
                foreach (AddonRepoGameConfig g in AddonRepoManager.Instance.Config.Games)
                {
                    tasks.Add(ReadGameInfoAsync(id: g.Name, name: g.Name, path: g.Path));
                }
            }

            // Steam
            if (AddonRepoManager.Instance.Config.Steam)
            {
                try
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
                                tasks.Add(ReadGameInfoAsync(uninstallEntry, subKey));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }

            foreach (Task<GameInfo> t in tasks.Where(t => t != null))
            {
                try
                {
                    GameInfo g = await t.ConfigureAwait(false);
                    if (g != null)
                    {
                        games.Add(g);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
            return games;
        }

        private static Task<GameInfo> ReadGameInfoAsync(RegistryKey uninstallEntry, string subKey)
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
            string id = Regex.Match(subKey, @"\d+", RegexOptions.Compiled).Value;
            return ReadGameInfoAsync(id: id, name: name, path: path);
        }

        private static async Task<GameInfo> ReadGameInfoAsync(string id, string name, string path)
        {

            if (string.IsNullOrEmpty(name))
            {
                name = Path.GetFileName(path);
            }
            string unityPlayerPath = Path.Combine(path, Constants.UnityPlayer);
            if (!File.Exists(unityPlayerPath))
            {
                return null;
            }
            string bieCoreLibPath = Path.Combine(path, "BepInEx", "core", "BepInEx.dll");
            GameInfo gameInfo = new()
            {
                Id = id,
                Name = name,
                Path = path,
                Is64Bit = await FileTool.Is64BitAsync(unityPlayerPath).ConfigureAwait(false),
                IsBIEInstalled = File.Exists(bieCoreLibPath),
                IsBIEInitialized = File.Exists(Path.Combine(path, "BepInEx", "config", "BepInEx.cfg")),
            };
            if (gameInfo.IsBIEInstalled)
            {
                gameInfo.BIEVersion = (await FileTool.ReadDllInfoAsync(bieCoreLibPath, versionOnly: true).ConfigureAwait(false))?.Version ?? string.Empty;
            }

            foreach (string exe in Directory.EnumerateFiles(path, "*.exe", SearchOption.TopDirectoryOnly))
            {
                if (!exe.Contains("unity", StringComparison.OrdinalIgnoreCase))
                {
                    using Icon icon = Icon.ExtractAssociatedIcon(exe);
                    using MemoryStream ms = new();
                    using (Bitmap bmp = icon.ToBitmap())
                    {
                        bmp.Save(ms, ImageFormat.Png);
                    }
                    gameInfo.Icon = $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}";
                    break;
                }
            }

            if (gameInfo.IsBIEInstalled)
            {
                foreach (PluginInfo pi in await InstallationUtils.ListPluginsAsync(path).ConfigureAwait(false))
                {
                    gameInfo.Plugins.Add(pi);
                }
            }

            string patcherDir = Path.Combine(Path.Combine(path, "BepInEx", "patchers"));
            if (Directory.Exists(patcherDir))
            {
                foreach (string patcher in Directory.EnumerateFiles(patcherDir, "*.dll", SearchOption.AllDirectories))
                {
                    PatcherInfo patchInfo = new()
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