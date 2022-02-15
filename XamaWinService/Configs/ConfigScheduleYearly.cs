using XamaCore.Configs;

namespace XamaWinService.Configs
{
    public class ConfigScheduleYearly : ConfigSchedule
    {
        public string Time { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
    }
}
