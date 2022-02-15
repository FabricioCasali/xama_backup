using System;

using XamaCore.Configs;
using XamaWinService.Configs;

namespace XamaWinService.Schedulers
{
    public class DailyScheduler : Scheduler<ConfigScheduleDaily>
    {
        public override bool IsCron()
        {
            return false;
        }

        public override DateTime NextRun(DateTime baseDate, ConfigScheduleDaily config)
        {
            ValidateConfig(config);
            var parts = config.Time.Split(':');
            var newDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), parts.Length > 2 ? Convert.ToInt32(parts[2]) : 0);
            if (baseDate >= newDate)
                return newDate.AddDays(1);
            return newDate;
        }

        public override void ValidateConfig(ConfigScheduleDaily config)
        {
            ValidateTime(config.Time);
        }
    }
}