using System;
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
                await CreateHostBuilder(port).Build().RunAsync().ConfigureAwait(false);
            }
            else // CLI mode
            {
#if DEBUG
                //await foreach (GameInfo x in ModManagerServiceImpl.GetSteamGamesAsync()) { }
#endif
                //var g = DotNet.Globbing.Glob.Parse("**/BepInEx.*.zip");
                //var m = g.IsMatch("http://a.com/BepInEx/BepInEx.Utility/releases/download/r7/BepInEx.CatchUnityEventExceptions.v1.0.zip");
                //Console.WriteLine(m);
                //await Repo.AddonRepoManager.Instance.UpdateBucketsAsync().ConfigureAwait(false);
                //var localPlugins = await Repo.AddonRepoManager.Instance.LoadLocalPluginsAsync().ConfigureAwait(false);
                //foreach (var p in localPlugins)
                //{
                //    Console.WriteLine(p.Path);
                //}
                await InstallationUtils.IntallPluginAsync(
                    gamePath: @"C:\Program Files (x86)\Steam\steamapps\common\觅长生",
                    pluginPath: @"C:\Users\harlo\.bierepo\plugins\BepInEx.EnableResize\BepInEx.EnableResize.1.5.dll")
                    .ConfigureAwait(false);
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
