using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CalendarReminder.Features.SendDailyReminder
{
    public class HttpTrigger
    {
        private readonly Feature _feature;
        public HttpTrigger( Feature feature) => _feature = feature;

        [Function("SendDailyReminderHttp")]
        public async Task Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            await _feature.SendDailyReminder();
        }
    }
}
