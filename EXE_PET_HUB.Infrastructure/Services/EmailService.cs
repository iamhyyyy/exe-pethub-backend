using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Threading;
using SendGrid;
using SendGrid.Helpers.Mail;

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
                if (_emailSettings == null)
                {
                    throw new InvalidOperationException("Email settings are not configured properly.");
                }

                // Ưu tiên dùng SendGrid nếu có API key (cho deploy, tránh bị block SMTP)
                if (!string.IsNullOrEmpty(_emailSettings.SendGridApiKey))
                {
                    await SendEmailViaSendGridAsync(toEmail, subject, body);
                    return;
                }

                // Fallback: dùng SMTP (cho local development)
                await SendEmailViaSmtpAsync(toEmail, subject, body);
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết để debug
                Console.WriteLine($"Email sending failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw; // Re-throw để caller biết có lỗi
            }
        }

        private async Task SendEmailViaSendGridAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrEmpty(_emailSettings.SendGridApiKey))
            {
                throw new InvalidOperationException("SendGrid API key is not configured.");
            }

            var client = new SendGridClient(_emailSettings.SendGridApiKey);
            var from = new EmailAddress(
                _emailSettings.SendGridFromEmail ?? _emailSettings.Mail, 
                _emailSettings.DisplayName ?? "PetHub System");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, body); // null = plain text, body = HTML

            var response = await client.SendEmailAsync(msg);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Body.ReadAsStringAsync();
                throw new Exception($"SendGrid API error: {response.StatusCode} - {errorBody}");
            }

            Console.WriteLine($"Email sent via SendGrid to {toEmail}");
        }

        private async Task SendEmailViaSmtpAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrEmpty(_emailSettings.Mail) || string.IsNullOrEmpty(_emailSettings.Host))
            {
                throw new InvalidOperationException("SMTP settings are not configured properly.");
            }

            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = body;
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Timeout = 30000;
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            
            // Thử port 587 với StartTls trước, nếu fail thì thử port 465 với SSL
            try
            {
                await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, 
                    _emailSettings.Port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls, 
                    cts.Token);
            }
            catch (Exception connectEx)
            {
                // Nếu port 587 fail, thử port 465 với SSL
                if (_emailSettings.Port == 587)
                {
                    Console.WriteLine($"Port 587 failed, trying port 465 with SSL: {connectEx.Message}");
                    await smtp.ConnectAsync(_emailSettings.Host, 465, SecureSocketOptions.SslOnConnect, cts.Token);
                }
                else
                {
                    throw;
                }
            }
            
            await smtp.AuthenticateAsync(_emailSettings.Mail, _emailSettings.Password, cts.Token);
            await smtp.SendAsync(email, cts.Token);
            await smtp.DisconnectAsync(true, cts.Token);
            
            Console.WriteLine($"Email sent via SMTP to {toEmail}");
        }
    }
}
