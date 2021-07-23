using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BepInEx.ModManager.Shared;
using Newtonsoft.Json;

namespace BepInEx.ModManager.DllReader
{
    public static class Program
    {
        public static void Main(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{path}");
            }

            var appDomain = AppDomain.CurrentDomain;
            var assemblyName = AssemblyName.GetAssemblyName(path);
            var assembly = appDomain.Load(assemblyName);
            try
            {
                foreach (var t in assembly.DefinedTypes)
                {
                    var pluginAttribute = t.GetCustomAttribute<BepInPlugin>();
                    if (pluginAttribute != null)
                    {
                        var pluginInfo = new BepInExAssemblyInfo
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
                foreach (var e in rtle.LoaderExceptions)
                {
                    Console.Error.WriteLine(e);
                }
#endif
            }
        }
    }
}
