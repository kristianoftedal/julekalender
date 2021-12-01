using System;
using System.Threading.Tasks;
using CalendarReminder.Infrastructure;
using Microsoft.Azure.Functions.Worker;

namespace CalendarReminder.Features.SendDailyReminder
{
    public class TimerTrigger
    {
        private readonly Feature _feature;
        public TimerTrigger(Feature feature) => _feature = feature;

        [Function("SendDailyReminderTimer")]
        public async Task Run([TimerTrigger("0 0 8 * * *")] MyInfo myTimer)
        {
            if (DateTime.Now.Month != 12)
                return;

            if (DateTime.Now.Day > 22)
                return;

            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                return;

            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                return;

            await _feature.SendDailyReminder();
        }
    }
}
