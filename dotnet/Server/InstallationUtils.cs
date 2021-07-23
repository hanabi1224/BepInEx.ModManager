using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace BepInEx.ModManager.Server
{
    public static class InstallationUtils
    {
        private static readonly HttpClient s_client = new HttpClient();
        public static async Task<string> GetLatestBIEDownloadUrlAsync(bool is64bit)
        {
            var html = new HtmlDocument();
            html.Load(await s_client.GetStreamAsync(Constants.BepInExLatestReleasePage).ConfigureAwait(false));
            var anchors = html.DocumentNode.SelectNodes("//a");
            var toContain = is64bit ? "BepInEx_x64" : "BepInEx_x86";
            var href = anchors.Select(a => a.GetAttributeValue("href", string.Empty)).FirstOrDefault(h => h.Contains(".zip", StringComparison.OrdinalIgnoreCase) && h.Contains(toContain, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(href) && href.StartsWith("/"))
            {
                var ub = new UriBuilder(Constants.BepInExLatestReleasePage);
                ub.Path = href;
                href = ub.ToString();
            }

            return href;
        }

        public static async Task InstallBIEAsync(string path, bool is64bit)
        {
            var zipUrl = await GetLatestBIEDownloadUrlAsync(is64bit).ConfigureAwait(false);
            var zipStream = await s_client.GetStreamAsync(zipUrl).ConfigureAwait(false);
            using var zip = new ZipArchive(zipStream);
            foreach (var entry in zip.Entries)
            {
                var targetPath = Path.Combine(path, entry.FullName);
                var targetPathDir = Path.GetDirectoryName(targetPath);
                if (!Directory.Exists(targetPathDir))
                {
                    Directory.CreateDirectory(targetPathDir);
                }
                using var fileStream = File.OpenWrite(targetPath);
                using var entryStream = entry.Open();                
                await entryStream.CopyToAsync(fileStream).ConfigureAwait(false);
            }
        }
    }
}
