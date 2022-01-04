using System;

using XamaCore.Configs;

namespace XamaWinService.Schedulers
{
    public class RunOnceScheduler : Scheduler<ConfigScheduleOnce>
    {
        /// <summary> Theres no need to create a next schedule, so this method only returns the input date </summary>
        public override DateTime NextRun(DateTime baseDate, ConfigScheduleOnce config)
        {
            return baseDate;
        }

        /// <summary> Don't do nothing because it's a date time, so its not possible to set and invalid datetime (as far i'm aware of)</summary>
        public override void ValidateConfig(ConfigScheduleOnce config)
        {
        }
    }
}