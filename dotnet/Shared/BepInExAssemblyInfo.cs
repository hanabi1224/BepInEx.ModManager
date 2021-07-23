using System;
using System.Collections.Generic;
using System.Text;

namespace BepInEx.ModManager.Shared
{
    public enum BepInExAssemblyType
    {
        Unknown = 0,
        Plugin = 1,
        Patcher = 2,
    }

    public class BepInExAssemblyInfo
    {
        public BepInExAssemblyType Type { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }
    }
}
