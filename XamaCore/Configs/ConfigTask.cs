using System.Collections.Generic;
using Newtonsoft.Json;

namespace XamaCore.Configs
{
    public class ConfigTask
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("paths")]
        public IList<ConfigPath> Paths { get; set; }

        [JsonProperty("globalIncludes")]
        public IList<ConfigPattern> GlobalInclude { get; set; }

        [JsonProperty("globalExcludes")]
        public IList<ConfigPattern> GlobalExclude { get; set; }

        public IList<ConfigSchedule> Schedules { get; set; }

        [JsonProperty("target")]
        public string Target { get; set; }

        [JsonProperty("useShadowCopy")]
        public bool UseVolumeShadowCopy { get; set; }
    }
}
