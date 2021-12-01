using System;
using CalendarReminder.Features.SendDailyReminder;
using CalendarReminder.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SendGrid.Extensions.DependencyInjection;

namespace CalendarReminder
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("local.settings.json", true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddScoped<Feature>();

                    services.AddSendGrid(options =>
                    {
                        options.ApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
                    });

                    services.AddSingleton(new DbConnectionFactory(Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")));
                })
                .Build();

            host.Run();
        }
    }
}