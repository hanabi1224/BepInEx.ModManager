using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace BepInEx.ModManager.Server
{
    internal static class RegistryUtility
    {
        public static T GetValue<T>(this RegistryKey baseKey, string subKey, string name)
        {
            var entry = baseKey.OpenSubKey(subKey);
            if (entry != null)
            {
                return (T)entry.GetValue(name);
            }
            return default;
        }
    }
}
