using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BepInEx.ModManager.Shared;
using Newtonsoft.Json;
using NLog;

namespace BepInEx.ModManager.Server
{
    public static class FileTool
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        public static async Task<bool> Is64BitAsync(string file)
        {
            string pwd = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "file", "bin");
            string exe = Path.Combine(pwd, "file.exe");
            if (!File.Exists(exe))
            {
                throw new InvalidOperationException("file tool not found.");
            }
            ProcessStartInfo psi = new ProcessStartInfo
            {
                WorkingDirectory = pwd,
                FileName = exe,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            psi.ArgumentList.Add(file);

            using Process p = Process.Start(psi);
            await p.WaitForExitAsync().ConfigureAwait(false);
            string stdout = await p.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
            string stderr = await p.StandardError.ReadToEndAsync().ConfigureAwait(false);
            return !string.IsNullOrEmpty(stdout) && !stdout.Contains("32-bit", StringComparison.OrdinalIgnoreCase);
        }

        public static async Task<BepInExAssemblyInfo> ReadDllInfoAsync(string file, bool versionOnly = false)
        {
            string pwd = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dllreader");
            string exe = Path.Combine(pwd, "BepInEx.ModManager.DllReader.exe");
            if (!File.Exists(exe))
            {
                throw new InvalidOperationException("dllreader tool not found.");
            }
            ProcessStartInfo psi = new ProcessStartInfo
            {
                WorkingDirectory = pwd,
                FileName = exe,
                RedirectStandardOutput = true,
                RedirectStandardError = false,
            };
            psi.ArgumentList.Add("--path");
            psi.ArgumentList.Add(file);
            if (versionOnly)
            {
                psi.ArgumentList.Add("--version-only");
                psi.ArgumentList.Add("true");
            }

            try
            {
                using Process p = Process.Start(psi);
                await p.WaitForExitAsync().ConfigureAwait(false);
                string stdout = await p.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
                if (!string.IsNullOrEmpty(stdout))
                {
                    return JsonConvert.DeserializeObject<BepInExAssemblyInfo>(stdout);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return new BepInExAssemblyInfo { Type = BepInExAssemblyType.Unknown };
        }
    }
}
