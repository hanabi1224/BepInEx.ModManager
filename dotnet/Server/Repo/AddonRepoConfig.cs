using System.Collections.Generic;
using Newtonsoft.Json;

namespace BepInEx.ModManager.Server.Repo
{
    public class AddonRepoConfig
    {
        public const string HelpText = @"
# format
# game:
#   - name: foo
#     path: c:\games\foo
#   - name: bar
#     path: c:\games\bar
#   ...
# buckets:
#   - name: BepInEx.Utility
#     url: https://github.com/BepInEx/BepInEx.Utility/releases/latest
#     patterns:
#       - BepInEx.*.zip
#   - name: BepInEx.ConfigurationManager
#     url: https://github.com/BepInEx/BepInEx.ConfigurationManager/releases/latest
#     patterns:
#       - BepInEx.*.zip
#   ...
# Patterns field can be ommited when url is a zip plugin archive or dll binary or an index json/yaml
# Index files should be a list of buckets
";

        public static AddonRepoConfig Default { get; } = new AddonRepoConfig
        {
            Games = new List<AddonRepoGameConfig>(),
            Buckets = new List<AddonRepoBucketConfig>
            {
                new AddonRepoBucketConfig
                {
                    Name= "BepInEx.Utility",
                    Url="https://github.com/BepInEx/BepInEx.Utility/releases/latest",
                    Patterns = new List<string>
                    {
                        "**/BepInEx.*.zip"
                    },
                },
                new AddonRepoBucketConfig
                {
                    Name= "BepInEx.ConfigurationManager",
                    Url="https://github.com/BepInEx/BepInEx.ConfigurationManager/releases/latest",
                    Patterns = new List<string>
                    {
                        "**/BepInEx.*.zip"
                    },
                },
            },
        };

        public List<AddonRepoGameConfig> Games { get; set; }

        public List<AddonRepoBucketConfig> Buckets { get; set; }
    }

    public class AddonRepoGameConfig
    {
        public string Name { get; set; }

        public string Path { get; set; }
    }

    public class AddonRepoBucketConfig
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]

        public string Url { get; set; }

        [JsonProperty("patterns")]

        public List<string> Patterns { get; set; }
    }
}
