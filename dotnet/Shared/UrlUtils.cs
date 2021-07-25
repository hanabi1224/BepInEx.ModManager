using System;
using System.Collections.Generic;
using System.Text;

namespace BepInEx.ModManager.Shared
{
    public static class UrlUtils
    {
        public static string GetAbsolutionUrl(string url, string referer)
        {
            bool isAbsolute = url.StartsWith("http://") || url.StartsWith("https://");
            return isAbsolute ? url : new UriBuilder(referer ?? string.Empty)
            {
                Path = url
            }.ToString();
        }
    }
}
