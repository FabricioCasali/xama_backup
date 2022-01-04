using System;

using XamaCore.Configs;

namespace XamaWinService.Schedulers
{
    public class WeeklyScheduler : Scheduler<ConfigScheduleWeekly>
    {
        public override DateTime NextRun(DateTime baseDate, ConfigScheduleWeekly config)
        {
            ValidateConfig(config);
            var parts = config.Time.Split(':');
            var newDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), parts.Length > 2 ? Convert.ToInt32(parts[2]) : 0);
            var lCount = 0;
            do
            {
                lCount++;
                var d = newDate.AddDays(lCount);
                if (d.DayOfWeek == DayOfWeek.Monday && config.Monday)
                    return d;
                if (d.DayOfWeek == DayOfWeek.Tuesday && config.Tuesday)
                    return d;
                if (d.DayOfWeek == DayOfWeek.Wednesday && config.Wednesday)
                    return d;
                if (d.DayOfWeek == DayOfWeek.Thursday && config.Thursday)
                    return d;
                if (d.DayOfWeek == DayOfWeek.Friday && config.Friday)
                    return d;
                if (d.DayOfWeek == DayOfWeek.Saturday && config.Saturday)
                    return d;
                if (d.DayOfWeek == DayOfWeek.Sunday && config.Sunday)
                    return d;
            } while (lCount <= 7);
            throw new SchedulerException("Could not find a valid date");
        }

        public override void ValidateConfig(ConfigScheduleWeekly config)
        {
            ValidateTime(config.Time);
            if (config.Monday == false && config.Tuesday == false &&
                config.Wednesday == false && config.Thursday == false &&
                config.Friday == false && config.Saturday == false && config.Sunday == false)
                throw new SchedulerException("Weekly scheduler could not find a day to run");
        }
    }
}