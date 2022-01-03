using System.Collections.Generic;
using Newtonsoft.Json;

namespace XamaCore.Configs
{
    public class ConfigApp
    {
        public ConfigApp()
        {
            Tasks = new List<ConfigTask>();
        }
        [JsonProperty("tasks")]
        public IList<ConfigTask> Tasks { get; set; }

        [JsonProperty("enableLog")]
        public bool EnableLog { get; set; }

        [JsonProperty("logConfig")]
        public ConfigLog LogConfig { get; set; }
    }
}
