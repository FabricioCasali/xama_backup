using System;

using XamaCore.Configs;

namespace XamaWinService.Schedulers
{
    public class YearlyScheduler : Scheduler<ConfigScheduleYearly>
    {
        public override DateTime NextRun(DateTime baseDate, ConfigScheduleYearly config)
        {
            ValidateConfig(config);
            var parts = config.Time.Split(':');
            //var newDate = new DateTime(baseDate.Year + 1, config.Month, config.Day, Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), parts.Length > 2 ? Convert.ToInt32(parts[2]) : 0);
            var y = baseDate.Year + 1;
            var m = config.Month;
            var d = config.Day;
            DateTime? newDate = null;
            var i = 0;
            while (i <= 4)
            {
                try
                {
                    newDate = new DateTime(y, m, d - i, Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), parts.Length > 2 ? Convert.ToInt32(parts[2]) : 0);
                    break;
                }
                catch (Exception)
                {
                    i++;
                }
            }
            if (newDate == null)
                throw new SchedulerException("Could not find a valid date");
            return newDate.Value;
        }

        public override void ValidateConfig(ConfigScheduleYearly config)
        {
            ValidateTime(config.Time);
            if (config.Month < 1 || config.Month > 12)
                throw new SchedulerException("Yearly scheduler could not find a month to run");
            if (config.Month < 1 || config.Month > 31)
                throw new SchedulerException("Yearly scheduler could not find a day to run");
        }
    }
}