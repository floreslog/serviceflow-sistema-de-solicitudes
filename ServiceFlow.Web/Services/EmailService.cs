using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ServiceFlow.Web.Services
{
    public class EmailService
    {
        private readonly IConfiguration config;

        public EmailService(IConfiguration config)
        {
            this.config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var settings = config.GetSection("EmailSettings");

            var fromName = settings["FromName"] ?? throw new InvalidOperationException("EmailSettings:FromName no configurado.");
            var fromEmail = settings["FromEmail"] ?? throw new InvalidOperationException("EmailSettings:FromEmail no configurado.");
            var host = settings["Host"] ?? throw new InvalidOperationException("EmailSettings:Host no configurado.");
            var port = int.Parse(settings["Port"] ?? throw new InvalidOperationException("EmailSettings:Port no configurado."));
            var username = settings["Username"] ?? throw new InvalidOperationException("EmailSettings:Username no configurado.");
            var password = settings["Password"] ?? throw new InvalidOperationException("EmailSettings:Password no configurado.");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(username, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}