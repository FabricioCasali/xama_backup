using System;

using XamaCore.Configs;

namespace XamaWinService.Schedulers
{
    public class MonthlyScheduler : Scheduler<ConfigScheduleMonthly>
    {
        public override DateTime NextRun(DateTime baseDate, ConfigScheduleMonthly config)
        {
            ValidateConfig(config);
            var parts = config.Time.Split(':');
            var y = baseDate.Year;
            var m = baseDate.Month + 1;
            var d = baseDate.Day;

            if (m == 13)
            {
                m = 1;
                y++;
            }
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

        public override void ValidateConfig(ConfigScheduleMonthly config)
        {
            if (config.Day < 1 || config.Day > 31)
                throw new SchedulerException("Monthly scheduler could not find a day to run");
            ValidateTime(config.Time);
        }
    }
}