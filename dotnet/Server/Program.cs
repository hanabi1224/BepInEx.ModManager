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
                await foreach (GameInfo x in ModManagerServiceImpl.GetSteamGamesAsync()) { }
#endif
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
