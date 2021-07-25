using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Mono.Cecil;

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
        private const string PluginAttributeTypeFullName = "BepInEx.BepInPlugin";
        private const string PluginProcessAttributeTypeFullName = "BepInEx.BepInProcess";
        private static readonly ISet<string> s_precludeDependencyPatterns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            @"System(\..+)?",
            @"UnityEngine(\..+)?",
            @"Assembly-CSharp(\..+)?",
            @"0Harmony(\d+)?",
        };
        private static IReadOnlyList<Regex> s_precludeDependencyRegexList { get; } = s_precludeDependencyPatterns.Select(p => new Regex(p, RegexOptions.Compiled | RegexOptions.IgnoreCase)).ToList();
        private static readonly ISet<string> s_precludeDependencies = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "mscorlib",
            "BepInEx.BepIn4Patcher",
            "BepInEx",
            "Newtonsoft.Json",
        };

        public BepInExAssemblyType Type { get; set; }

        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Version { get; set; } = string.Empty;

        public string GameProcessName { get; set; } = string.Empty;

        public List<BepInExAssemblyReferenceInfo> References { get; set; } = new List<BepInExAssemblyReferenceInfo>();

        public static bool TryRead(string path, out BepInExAssemblyInfo pluginInfo, bool shallow)
        {
            if (!File.Exists(path))
            {
                pluginInfo = null;
                return false;
            }

            TempDir tempDir = null;
            pluginInfo = new();
            try
            {
                if (shallow)
                {
                    tempDir = new TempDir();
                    string newPath = Path.Combine(tempDir.Dir, Path.GetFileName(path));
                    File.Copy(path, newPath, true);
                    path = newPath;
                }

                using AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(path);
                AssemblyDefinition assembly = assemblyDefinition.MainModule.Assembly;
                Match match = Regex.Match(assembly.FullName, @"Version=(?<version>.+?),", RegexOptions.Compiled);
                if (match.Success && System.Version.TryParse(match.Groups["version"].Value.Trim(), out System.Version version))
                {
                    pluginInfo.Version = version.ToString();
                }

                if (assemblyDefinition.MainModule.AssemblyReferences?.Count > 0)
                {
                    foreach (AssemblyNameReference reference in assemblyDefinition.MainModule.AssemblyReferences)
                    {
                        if (!s_precludeDependencies.Contains(reference.Name))
                        {
                            if (!s_precludeDependencyRegexList.Any(r => r.IsMatch(reference.Name)))
                            {
                                pluginInfo.References.Add(new()
                                {
                                    Name = reference.Name,
                                    Version = reference.Version.ToString(),
                                });
                            }
                        }
                    }
                }

                foreach (CustomAttribute attr in assembly.CustomAttributes)
                {
                    switch (attr.AttributeType.FullName)
                    {
                        case "System.Reflection.AssemblyTitleAttribute":
                            if (attr.ConstructorArguments?.Count > 0)
                            {
                                pluginInfo.Id = pluginInfo.Name = attr.ConstructorArguments[0].Value as string;
                            }
                            break;
                        case "System.Reflection.AssemblyDescriptionAttribute":
                            if (attr.ConstructorArguments?.Count > 0)
                            {
                                pluginInfo.Description = attr.ConstructorArguments[0].Value as string;
                            }
                            break;
                        case "System.Reflection.AssemblyVersionAttribute":
                            break;
                        case "System.Reflection.AssemblyFileVersionAttribute":
                            if (attr.ConstructorArguments?.Count > 0 && string.IsNullOrEmpty(pluginInfo.Version))
                            {
                                pluginInfo.Version = attr.ConstructorArguments[0].Value as string;
                            }
                            break;
                    }
                }

                foreach (TypeDefinition t in assemblyDefinition.MainModule.Types)
                {
                    if (t.HasCustomAttributes)
                    {
                        foreach (CustomAttribute attr in t.CustomAttributes)
                        {

                            switch (attr.AttributeType.FullName)
                            {
                                case PluginAttributeTypeFullName:
                                    if (attr.ConstructorArguments?.Count >= 3)
                                    {
                                        pluginInfo.Type = BepInExAssemblyType.Plugin;
                                        pluginInfo.Id = attr.ConstructorArguments[0].Value as string;
                                        pluginInfo.Name = attr.ConstructorArguments[1].Value as string;
                                        pluginInfo.Version = attr.ConstructorArguments[2].Value as string;
                                    }
                                    break;
                                case PluginProcessAttributeTypeFullName:
                                    if (attr.ConstructorArguments?.Count >= 1)
                                    {
                                        pluginInfo.GameProcessName = attr.ConstructorArguments[0].Value as string;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                return !string.IsNullOrEmpty(pluginInfo.Id) && !string.IsNullOrEmpty(pluginInfo.Name) && !string.IsNullOrEmpty(pluginInfo.Version);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return false;
            }
            finally
            {
                if (tempDir != null)
                {
                    using (tempDir) { }
                }
            }
        }
    }

    public class BepInExAssemblyReferenceInfo
    {
        public string Name { get; set; }

        public string Version { get; set; }
    }
}
