using Newtonsoft.Json;

namespace XamaWinService.Configs
{
    public class ConfigLog
    {
        [JsonProperty("fileName")]
        public string LogFileName { get; set; }

        [JsonProperty("path")]
        public string LogFilePath { get; set; }

        [JsonProperty("maxSize")]
        public int MaxLogSize { get; set; }

        [JsonProperty("maxFiles")]
        public int MaxArchiveFiles { get; set; }

        [JsonProperty("showTrace")]
        public bool ShowTrace { get; set; }

    }
}
