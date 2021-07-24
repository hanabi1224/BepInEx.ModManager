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

        public static void Main(string path, bool versionOnly = false)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{path}");
            }

            Assembly assembly = null;
            if (!versionOnly)
            {
                try
                {
                    AssemblyName assemblyName = AssemblyName.GetAssemblyName(path);
                    assembly = AppDomain.CurrentDomain.Load(assemblyName);
                }
                catch
                {
                }
            }
            if (assembly == null)
            {
                assembly = Assembly.ReflectionOnlyLoadFrom(path);
            }

            if (!versionOnly)
            {
                try
                {
                    foreach (TypeInfo t in assembly.DefinedTypes)
                    {
                        BepInPlugin pluginAttribute = t.GetCustomAttribute<BepInPlugin>();
                        if (pluginAttribute != null)
                        {
                            BepInExAssemblyInfo pluginInfo = new()
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
                    // TODO: Detetct patcher infomation
                    //foreach (TypeInfo t in assembly.DefinedTypes)
                    //{
                    //    BepInExAssemblyInfo patcherInfo = new()
                    //    {
                    //        Type = BepInExAssemblyType.Patcher,
                    //        Id = pluginAttribute.GUID,
                    //        Name = pluginAttribute.Name,
                    //        Version = pluginAttribute.Version.ToString(),
                    //    };
                    //    Console.WriteLine(JsonConvert.SerializeObject(patcherInfo));
                    //    return;
                    //}
                    return;
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
            }
            {
                Match match = Regex.Match(assembly.FullName, @"Version=(?<version>.+?),", RegexOptions.Compiled);
                if (match.Success && Version.TryParse(match.Groups["version"].Value.Trim(), out Version version))
                {
                    string name = Path.GetFileNameWithoutExtension(path);
                    BepInExAssemblyInfo pluginInfo = new()
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
