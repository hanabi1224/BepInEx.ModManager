using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BepInEx.ModManager.Shared;
using Newtonsoft.Json;

namespace BepInEx.ModManager.Server
{
    public static class FileTool
    {
        public static async Task<bool> Is64BitAsync(string file)
        {
            var pwd = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "file", "bin");
            var exe = Path.Combine(pwd, "file.exe");
            if (!File.Exists(exe))
            {
                throw new InvalidOperationException("file tool not found.");
            }
            var psi = new ProcessStartInfo
            {
                WorkingDirectory = pwd,
                FileName = exe,
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

        public static async Task<BepInExAssemblyInfo> ReadDllInfoAsync(string file)
        {
            var pwd = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dllreader");
            var exe = Path.Combine(pwd, "BepInEx.ModManager.DllReader.exe");
            if (!File.Exists(exe))
            {
                throw new InvalidOperationException("dllreader tool not found.");
            }
            var psi = new ProcessStartInfo
            {
                WorkingDirectory = pwd,
                FileName = exe,
                RedirectStandardOutput = true,
                RedirectStandardError = false,
            };
            psi.ArgumentList.Add("--path");
            psi.ArgumentList.Add(file);

            try
            {
                using var p = Process.Start(psi);
                await p.WaitForExitAsync().ConfigureAwait(false);
                var stdout = await p.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
                if (!string.IsNullOrEmpty(stdout))
                {
                    return JsonConvert.DeserializeObject<BepInExAssemblyInfo>(stdout);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }

            return new BepInExAssemblyInfo { Type = BepInExAssemblyType.Unknown };
        }
    }
}
