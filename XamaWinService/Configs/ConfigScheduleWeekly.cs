using XamaCore.Configs;

namespace XamaWinService.Configs
{
    public class ConfigScheduleWeekly : ConfigSchedule
    {
        public ConfigScheduleWeekly()
        {
        }

        public string Time { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
    }
}
