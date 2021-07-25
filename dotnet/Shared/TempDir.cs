using System;
using System.IO;
using NLog;

namespace BepInEx.ModManager.Shared
{
    public class TempDir : DisposableBase
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        public static string TempFilesRoot { get; } = Path.Combine(Path.GetTempPath(), ".tempdir");

        public string Dir { get; }

        public TempDir()
        {
            Dir = Path.Combine(TempFilesRoot, Path.GetRandomFileName());
            if (!Directory.Exists(Dir))
            {
                Directory.CreateDirectory(Dir);
            }
        }

        protected override void DisposeResources()
        {
            try
            {
                Directory.Delete(Dir, true);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
