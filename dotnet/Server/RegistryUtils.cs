using Microsoft.Win32;

namespace BepInEx.ModManager.Server
{
    internal static class RegistryUtility
    {
        public static T GetValue<T>(this RegistryKey baseKey, string subKey, string name)
        {
            RegistryKey entry = baseKey.OpenSubKey(subKey);
            if (entry != null)
            {
                return (T)entry.GetValue(name);
            }
            return default;
        }
    }
}
