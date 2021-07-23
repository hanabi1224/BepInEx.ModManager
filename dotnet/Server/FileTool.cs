using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BepInEx.ModManager.Server
{
    public static class FileTool
    {
        public static async Task<bool> Is64BitAsync(string file)
        {
            var psi = new ProcessStartInfo
            {
                WorkingDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "file", "bin"),
                FileName = "file",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            psi.ArgumentList.Add(file);

            using var p = Process.Start(psi);
            await p.WaitForExitAsync().ConfigureAwait(false);
            var stdout = await p.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
            var stderr = await p.StandardError.ReadToEndAsync().ConfigureAwait(false);
            return !string.IsNullOrEmpty(stdout) && !stdout.Contains("32-bit", StringComparison.OrdinalIgnoreCase);
        }
    }
}
