using CalendarReminder.Features.SendDailyReminder;

namespace CalendarReminder.Infrastructure
{
    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }
}
