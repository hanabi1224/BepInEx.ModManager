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
            HtmlDocument html = new HtmlDocument();
            html.Load(await s_client.GetStreamAsync(Constants.BepInExLatestReleasePage).ConfigureAwait(false));
            HtmlNodeCollection anchors = html.DocumentNode.SelectNodes("//a");
            string toContain = is64bit ? "BepInEx_x64" : "BepInEx_x86";
            string href = anchors.Select(a => a.GetAttributeValue("href", string.Empty)).FirstOrDefault(h => h.Contains(".zip", StringComparison.OrdinalIgnoreCase) && h.Contains(toContain, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(href) && href.StartsWith("/"))
            {
                UriBuilder ub = new UriBuilder(Constants.BepInExLatestReleasePage)
                {
                    Path = href
                };
                href = ub.ToString();
            }

            return href;
        }

        public static async Task InstallBIEAsync(string path, bool is64bit)
        {
            string zipUrl = await GetLatestBIEDownloadUrlAsync(is64bit).ConfigureAwait(false);
            Stream zipStream = await s_client.GetStreamAsync(zipUrl).ConfigureAwait(false);
            using ZipArchive zip = new ZipArchive(zipStream);
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
    }
}
