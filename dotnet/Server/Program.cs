﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BepInEx.ModManager.Server.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog;

namespace BepInEx.ModManager.Server
{
    public static class Program
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        static Program()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public static async Task Main(
            bool server = false,
            int port = 40003)
        {
            if (server) // Server mode
            {
                _ = Task.Run(async () =>
                {
                    await Repo.AddonRepoManager.Instance.UpdateBucketsAsync().ConfigureAwait(false);
                    if (ClientNotification.ChannelWriter != null)
                    {
                        await ClientNotification.ChannelWriter.WriteAsync(new()
                        {
                            Notification = ServerSideNotification.RefreshRepoInfo,
                        }).ConfigureAwait(false);
                    }
                });
                await CreateHostBuilder(port).Build().RunAsync().ConfigureAwait(false);
            }
            else // CLI mode
            {
#if DEBUG
                //await foreach (GameInfo x in ModManagerServiceImpl.GetSteamGamesAsync()) { }
#endif
                await Repo.AddonRepoManager.Instance.UpdateBucketsAsync().ConfigureAwait(false);
            }
        }

        public static IHostBuilder CreateHostBuilder(int port)
        {
            return Host.CreateDefaultBuilder()
.ConfigureWebHostDefaults(webBuilder =>
{
    webBuilder
    .ConfigureKestrel(options =>
    {
        options.ListenLocalhost(port);
    })
    .UseKestrel()
    .UseStartup<Startup>();
});
        }
    }
}
