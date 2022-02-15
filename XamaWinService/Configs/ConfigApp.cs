using System.Collections.Generic;

using Newtonsoft.Json;

using XamaCore.Configs;

namespace XamaWinService.Configs
{
    public class ConfigApp
    {
        public ConfigApp()
        {
        }

        [JsonProperty("tasks")]
        public IList<ConfigTask> Tasks { get; set; }

        [JsonProperty("enableLog")]
        public bool EnableLog { get; set; }

        [JsonProperty("logConfig")]
        public ConfigLog LogConfig { get; set; }
    }
}
