﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        public static AddonRepoConfig Default { get; } = new()
        {
            Games = new(),
            Buckets = new()
            {
                new()
                {
                    Name = "BepInEx.Utility",
                    Url = "https://github.com/BepInEx/BepInEx.Utility/releases/latest",
                    Patterns = new()
                    {
                        "**/BepInEx.*.zip"
                    },
                },
                new()
                {
                    Name = "BepInEx.ConfigurationManager",
                    Url = "https://github.com/BepInEx/BepInEx.ConfigurationManager/releases/latest",
                    Patterns = new()
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

        public class EqualityComparer : IEqualityComparer<AddonRepoBucketConfig>
        {
            public static IEqualityComparer<AddonRepoBucketConfig> Instance { get; } = new EqualityComparer();

            public bool Equals(AddonRepoBucketConfig x, AddonRepoBucketConfig y)
            {
                return x.Url == y.Url;
            }

            public int GetHashCode([DisallowNull] AddonRepoBucketConfig obj)
            {
                return obj.Url.GetHashCode();
            }
        }
    }
}
