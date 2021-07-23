using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.ModManager.Server.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BepInEx.ModManager.Server
{
    public static class Program
    {
        static Program()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public static async Task Main(int port = 40003)
        {
#if DEBUG
            await foreach (var x in ModManagerServiceImpl.GetSteamGamesAsync()) { }
#endif
            await CreateHostBuilder(port).Build().RunAsync().ConfigureAwait(false);
        }

        public static IHostBuilder CreateHostBuilder(int port) =>
            Host.CreateDefaultBuilder()
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
