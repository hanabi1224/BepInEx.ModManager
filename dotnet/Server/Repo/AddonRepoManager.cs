using System;
using System.IO;
using NLog;

namespace BepInEx.ModManager.Server.Repo
{
    public class AddonRepoManager
    {
        private const string RepoDirName = ".bierepo";

        private const string ConfigFileName = "config.yaml";

        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        public static AddonRepoManager Instance { get; } = new AddonRepoManager();

        public string RootPath { get; } = GetRootPath();

        public string ConfigFilePath => Path.Combine(RootPath, ConfigFileName);

        private AddonRepoManager() { }

        private static string GetRootPath()
        {
            var root = Environment.GetEnvironmentVariable("BIE_REPO_ROOT");
            if (string.IsNullOrEmpty(root))
            {
                var parrentDir = Environment.GetEnvironmentVariable("USERPROFILE");
                if (string.IsNullOrEmpty(parrentDir))
                {
                    parrentDir = @"c:\";
                }

                root = Path.Combine(parrentDir, RepoDirName);
            }

            if (!Directory.Exists(root))
            {
                try
                {
                    Directory.CreateDirectory(root);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }

            return root;
        }
    }
}
