using System;
using XamaWinService.Configs;

namespace XamaWinService.Schedulers
{
    public class OnStartupScheduler : Scheduler<ConfigScheduleOnStartUp>
    {
        public override bool IsCron()
        {
            return false;
        }

        public override DateTime NextRun(DateTime baseDate, ConfigScheduleOnStartUp config)
        {
            return DateTime.Now.AddSeconds(10);
        }

        public override void ValidateConfig(ConfigScheduleOnStartUp config)
        {
        }
    }
}