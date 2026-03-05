using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Options;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Infrastructure.Settings;

namespace EXE_PET_HUB.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly SendGridSettings _settings;

        public EmailService(IOptions<SendGridSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var client = new SendGridClient(_settings.ApiKey);

            var from = new EmailAddress(_settings.FromEmail, _settings.FromName);
            var to = new EmailAddress(toEmail);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);

            await client.SendEmailAsync(msg);
        }
    }
}
