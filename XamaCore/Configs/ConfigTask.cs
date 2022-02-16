using System.Collections.Generic;

using Newtonsoft.Json;

namespace XamaCore.Configs
{
    public class ConfigTask
    {
        public ConfigTask()
        {
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("paths")]
        public IList<ConfigPath> Paths { get; set; }

        [JsonProperty("schedules")]
        public IList<ConfigSchedule> Schedules { get; set; }

        [JsonProperty("target")]
        public ConfigTarget Target { get; set; }

        [JsonProperty("type")]
        public ConfigTaskTypeEnum TaskType { get; set; }
    }
}
