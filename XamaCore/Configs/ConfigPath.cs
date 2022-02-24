using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace XamaCore.Configs
{
    public class ConfigPath
    {
        public ConfigPath()
        {
            IncludeSubfolders = true;
        }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("includesSubfolders")]
        public bool IncludeSubfolders { get; set; }

        [JsonProperty("includes")]
        public IList<ConfigPattern> Includes { get; set; }

        [JsonProperty("excludes")]
        public IList<ConfigPattern> Excludes { get; set; }

        [Obsolete("Not implemented yet", true)]
        [JsonProperty("useShadowCopy")]
        public bool UseVolumeShadowCopy { get; set; }
    }
}
