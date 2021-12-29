using Newtonsoft.Json;

namespace XamaCore.Configs
{
    public class ConfigPattern
    {
        public ConfigPattern()
        {
            this.Mode = ConfigPatternMode.Name;
            this.PatternType = ConfigPatternTypeEnum.Wildcard;
            this.ApplyesTo = ConfigPatternFileType.Both;
        }

        [JsonProperty("pattern")]
        public string Pattern { get; set; }

        [JsonProperty("type")]
        public ConfigPatternTypeEnum PatternType { get; set; }

        [JsonProperty("mode")]
        public ConfigPatternMode Mode { get; set; }

        [JsonProperty("applyesTo")]
        public ConfigPatternFileType ApplyesTo { get; set; }

        public override string ToString()
        {
            return $"{this.PatternType.ToString()} - {this.Pattern}, mode: {this.Mode.ToString()}, applyesTo: {this.ApplyesTo.ToString()}";
        }
    }
}
