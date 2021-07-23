using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using BepInEx.ModManager.Shared;
using Newtonsoft.Json;

namespace BepInEx.ModManager.DllReader
{
    public static class Program
    {
        static Program()
        {
            _ = typeof(BepInPlugin);
        }

        public static void Main(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{path}");
            }

            AppDomain appDomain = AppDomain.CurrentDomain;
            Assembly assembly;
            try
            {
                AssemblyName assemblyName = AssemblyName.GetAssemblyName(path);
                assembly = appDomain.Load(assemblyName);
            }
            catch
            {
                assembly = Assembly.ReflectionOnlyLoadFrom(path);
            }
            try
            {
                foreach (TypeInfo t in assembly.DefinedTypes)
                {
                    BepInPlugin pluginAttribute = t.GetCustomAttribute<BepInPlugin>();
                    if (pluginAttribute != null)
                    {
                        BepInExAssemblyInfo pluginInfo = new BepInExAssemblyInfo
                        {
                            Type = BepInExAssemblyType.Plugin,
                            Id = pluginAttribute.GUID,
                            Name = pluginAttribute.Name,
                            Version = pluginAttribute.Version.ToString(),
                        };
                        Console.WriteLine(JsonConvert.SerializeObject(pluginInfo));
                        return;
                    }
                }
            }
            catch (ReflectionTypeLoadException rtle)
            {
#if DEBUG
                foreach (Exception e in rtle.LoaderExceptions)
                {
                    Console.Error.WriteLine(e);
                }
#endif
            }
            catch (Exception e)
            {
#if DEBUG
                Console.Error.WriteLine(e);
#endif
            }
            {
                Match match = Regex.Match(assembly.FullName, @"Version=(?<version>.+?),", RegexOptions.Compiled);
                if (match.Success && Version.TryParse(match.Groups["version"].Value.Trim(), out Version version))
                {
                    string name = Path.GetFileNameWithoutExtension(path);
                    BepInExAssemblyInfo pluginInfo = new BepInExAssemblyInfo
                    {
                        Type = BepInExAssemblyType.Unknown,
                        Id = name,
                        Name = name,
                        Version = version.ToString(),
                    };
                    Console.WriteLine(JsonConvert.SerializeObject(pluginInfo));
                }
            }
        }
    }
}
