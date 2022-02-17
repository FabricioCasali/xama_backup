using Newtonsoft.Json;

using XamaCore.Configs;

namespace XamaWinService.Configs
{
    public class ConfigScheduleDaily : ConfigSchedule
    {
        [JsonProperty("time")]
        public string Time { get; set; }
    }
}
