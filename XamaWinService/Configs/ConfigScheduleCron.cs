using XamaCore.Configs;

namespace XamaWinService.Configs
{
    public class ConfigScheduleCron : ConfigSchedule
    {
        public ConfigScheduleCron()
        {
        }

        public string CronExpression { get; set; }
    }
}
