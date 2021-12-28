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

    }
}
