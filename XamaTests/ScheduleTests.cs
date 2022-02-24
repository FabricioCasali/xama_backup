using System;
using System.IO;

using FluentAssertions;

using XamaWinService.Configs;
using XamaWinService.Schedulers;

using Xunit;

namespace XamaTests
{
    public class ScheduleTests
    {
        [Fact]
        public void Schedule_Daily()
        {
            var scheduler = new DailyScheduler();
            var baseDate = new DateTime(2022, 1, 1, 12, 0, 0);
            scheduler.NextRun(baseDate, new ConfigScheduleDaily() { Time = "12:00:00" }).Should().Be(new DateTime(2022, 1, 2, 12, 0, 0));
        }

        [Fact]
        public void Schedule_Daily_InvalidHour()
        {
            var scheduler = new DailyScheduler();
            var baseDate = new DateTime(2022, 1, 1);
            Assert.Throws<SchedulerException>(() => scheduler.NextRun(baseDate, new ConfigScheduleDaily() { Time = "32:00:00" }));
        }

        [Fact]
        public void Scheduler_Monthly()
        {
            var s = new MonthlyScheduler();
            var baseDate = new DateTime(2022, 1, 1);
            s.NextRun(baseDate, new ConfigScheduleMonthly() { Time = "12:00:00", Day = 1 }).Should().Be(new DateTime(2022, 2, 1, 12, 0, 0));
        }

        [Fact]
        public void Scheduler_Monthly_LastDayOfMonth()
        {
            var s = new MonthlyScheduler();
            var baseDate = new DateTime(2022, 1, 31);
            s.NextRun(baseDate, new ConfigScheduleMonthly() { Time = "12:00:00", Day = 31 }).Should().Be(new DateTime(2022, 2, 28, 12, 0, 0));
        }

        [Fact]
        public void Scheduler_Monthly_InvalidParameter()
        {
            var s = new MonthlyScheduler();
            var baseDate = new DateTime(2022, 1, 1);
            Assert.Throws<SchedulerException>(() => s.NextRun(baseDate, new ConfigScheduleMonthly() { Time = "12:00:00", Day = 55 }));
            Assert.Throws<SchedulerException>(() => s.NextRun(baseDate, new ConfigScheduleMonthly() { Time = "XX", Day = 1 }));
            Assert.Throws<SchedulerException>(() => s.NextRun(baseDate, new ConfigScheduleMonthly() { Time = "25:00", Day = 1 }));
            Assert.Throws<SchedulerException>(() => s.NextRun(baseDate, new ConfigScheduleMonthly() { Time = "12:00:00", Day = 0 }));
        }

        [Fact]
        public void Scheduler_Weekly()
        {
            var s = new WeeklyScheduler();
            var baseDate = new DateTime(2022, 1, 1);
            s.NextRun(baseDate, new ConfigScheduleWeekly() { Time = "12:00:00", Monday = true }).Should().Be(new DateTime(2022, 1, 3, 12, 0, 0));
        }

        [Fact]
        public void Scheduler_Weekly_NoDaySelected()
        {
            var s = new WeeklyScheduler();
            var baseDate = new DateTime(2022, 1, 31);
            Assert.Throws<SchedulerException>(() => s.NextRun(baseDate, new ConfigScheduleWeekly() { Time = "12:00:00" }));
        }

        [Fact]
        public void Scheduler_Weekly_InvalidParameter()
        {
            var s = new WeeklyScheduler();
            var baseDate = new DateTime(2022, 1, 1);
            Assert.Throws<SchedulerException>(() => s.NextRun(baseDate, new ConfigScheduleWeekly() { Time = "1200" }));
            Assert.Throws<SchedulerException>(() => s.NextRun(baseDate, new ConfigScheduleWeekly() { Time = "XXX" }));
            Assert.Throws<SchedulerException>(() => s.NextRun(baseDate, new ConfigScheduleWeekly() { Time = "00:00:59" })); // valid time, but no day to run

        }
    }
}
