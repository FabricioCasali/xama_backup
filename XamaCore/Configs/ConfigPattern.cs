using Newtonsoft.Json;

namespace XamaCore.Configs
{
    public class ConfigPattern
    {
        [JsonProperty("pattern")]
        public string Pattern { get; set; }

        [JsonProperty("type")]
        public ConfigPatternTypeEnum PatternType { get; set; }
    }
}
