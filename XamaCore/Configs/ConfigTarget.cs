using Newtonsoft.Json;

namespace XamaCore.Configs
{
    public class ConfigTarget
    {
        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("compressionMethod")]
        public CompressionMethod CompressionMethod { get; set; }


    }
}
