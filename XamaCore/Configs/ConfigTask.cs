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
        public ConfigTarget Target { get; set; }

        [JsonProperty("useShadowCopy")]
        public bool UseVolumeShadowCopy { get; set; }
    }

    public class ConfigTarget
    {
        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("compressionMethod")]
        public CompressionMethod CompressionMethod { get; set; }


    }

    public enum CompressionMethod
    {
        SevenZip = 1,
    }
}
