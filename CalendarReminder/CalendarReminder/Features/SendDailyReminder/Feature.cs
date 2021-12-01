using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarReminder.Infrastructure;
using Dapper;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CalendarReminder.Features.SendDailyReminder
{
    public class Feature
    {
        private readonly ISendGridClient _client;
        private readonly DbConnectionFactory _dbConnectionFactory;

        public Feature(ISendGridClient client, DbConnectionFactory dbConnectionFactory)
        {
            _client = client;
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task SendDailyReminder()
        {
            var emailAddresses = await GetEmailAddresses();

            if (!emailAddresses.Any())
                return;

            var msg = new SendGridMessage()
            {
                From = new EmailAddress("hallstein.brotan@novanet.no", "Novanet Julekalender"),
                Subject = "Daglig påminnelse om kalenderluke",
                ReplyTo = new EmailAddress("no-reply@novanet.no")
            };

            msg.AddTo(new EmailAddress("no-reply@novanet.no"));

            msg.AddContent(MimeType.Html, "Ny luke på <a href=\"https://julekalender.novanet.no\">https://julekalender.novanet.no</a>. Der kan man også melde seg av påminnelsene.<br><br>Mvh<br>Novanet AS");

            foreach (var emailAddress in emailAddresses)
            {
                msg.AddBcc(new EmailAddress(emailAddress));
            }
            msg.AddBcc(new EmailAddress("hallstein.brotan@novanet.no"));
            var response = await _client.SendEmailAsync(msg);
        }

        public async Task<IList<string>> GetEmailAddresses()
        {
            using var connection = _dbConnectionFactory.Connection;

            var sql = @"
                SELECT TOP 1 1
                FROM
                    [dbo].[Door] d  
                WHERE 
	                YEAR(d.ForDate) = YEAR(GETDATE()) AND
	                MONTH(d.ForDate) = MONTH(GETDATE()) AND
	                DAY(d.ForDate) = DAY(GETDATE())";

            var doorExists = await connection.ExecuteScalarAsync<bool>(sql);

            if (!doorExists)
                return new List<string>();

            sql = @"SELECT DISTINCT u.Email FROM [dbo].[AspNetUsers] u WHERE u.WantsDailyNotification = 1";

            return (await connection.QueryAsync<string>(sql)).ToList();
        }
    }
}
