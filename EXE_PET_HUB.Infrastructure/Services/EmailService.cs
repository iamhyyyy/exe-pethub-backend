using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace EXE_PET_HUB.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                if (_emailSettings == null || string.IsNullOrEmpty(_emailSettings.Mail))
                {
                    throw new InvalidOperationException("Email settings are not configured properly.");
                }

                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_emailSettings.Mail);
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = body;
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.Mail, _emailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết để debug
                Console.WriteLine($"Email sending failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw; // Re-throw để caller biết có lỗi
            }
        }
    }
}
