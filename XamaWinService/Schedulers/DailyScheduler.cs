using System.Linq;
using System.Security.AccessControl;
using System;

using XamaCore.Configs;

namespace XamaWinService.Schedulers
{
    public class DailyScheduler : Scheduler<ConfigScheduleDaily>
    {
        public override DateTime NextRun(DateTime baseDate, ConfigScheduleDaily config)
        {
            ValidateConfig(config);
            var parts = config.Time.Split(':');
            var newDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), parts.Length > 2 ? Convert.ToInt32(parts[2]) : 0).AddDays(1);
            return newDate;
        }

        public override void ValidateConfig(ConfigScheduleDaily config)
        {
            ValidateTime(config.Time);
        }
    }
}