using Newtonsoft.Json;

namespace XamaCore.Configs
{
    public class ConfigTarget
    {
        public ConfigTarget()
        {
            CompressionLevel = ConfigCompressionLevel.Ultra;
        }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("compressionMethod")]
        public ConfigCompressionMethod CompressionMethod { get; set; }

        [JsonProperty("compressionLevel")]
        public ConfigCompressionLevel CompressionLevel { get; set; }
    }
}
