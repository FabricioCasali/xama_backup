using System;

using XamaCore.Configs;

namespace XamaWinService.Schedulers
{
    public abstract class Scheduler<T> : IScheduler<T> where T : ConfigSchedule
    {
        public abstract bool IsCron();
        public abstract DateTime NextRun(DateTime baseDate, T config);

        public DateTime NextRun(DateTime baseDate, object config)
        {
            return NextRun(baseDate, (T)config);
        }

        public abstract void ValidateConfig(T config);

        public void ValidateConfig(object config)
        {
            ValidateConfig((T)config);
        }

        protected virtual void ValidateTime(string t)
        {
            if (string.IsNullOrWhiteSpace(t))
                throw new SchedulerException("Time is not valid");

            var part = t.Split(':');
            if (part.Length < 2 || part.Length > 3)
                throw new SchedulerException("Time is not valid");

            var valid = Int32.TryParse(part[0], out int i);
            if (!valid)
                throw new SchedulerException("Time is not valid");

            if (i < 0 || i > 23)
                throw new SchedulerException("Time is not valid");

            valid = Int32.TryParse(part[1], out i);
            if (!valid)
                throw new SchedulerException("Time is not valid");

            if (i < 0 || i > 59)
                throw new SchedulerException("Time is not valid");

            if (part.Length > 2)
            {
                valid = Int32.TryParse(part[2], out i);
                if (!valid)
                    throw new SchedulerException("Time is not valid");

                if (i < 0 || i > 59)
                    throw new SchedulerException("Time is not valid");
            }
        }
    }
}