using System;
using XamaWinService.Configs;

namespace XamaWinService.Schedulers
{
    public class CronScheduler : Scheduler<ConfigScheduleCron>
    {
        public override bool IsCron()
        {
            return true;
        }

        public override DateTime NextRun(DateTime baseDate, ConfigScheduleCron config)
        {
            throw new NotImplementedException();
        }

        public override void ValidateConfig(ConfigScheduleCron config)
        {
            throw new NotImplementedException();
        }
    }

}