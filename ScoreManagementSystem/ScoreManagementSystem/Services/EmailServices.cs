
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ScoreManagementSystem.Services
{
    public class EmailServices
    {
        private readonly EmailConfiguration _emailConfiguration;

        public EmailServices()
        {
            _emailConfiguration = new EmailConfiguration
            {
                SmtpServer = "smtp.gmail.com",
                Port = 587,
                From = "ngkien112@gmail.com",
                SmtpUsername = "ngkien112",
                SmtpPassword = "ajwugthafavbqbas"
            };
        }

        public async Task SendAsync(EmailMessage emailMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Score System", _emailConfiguration.From));
            message.To.Add(new MailboxAddress("Recipient Name", emailMessage.To));
            message.Subject = emailMessage.Subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = emailMessage.Content };
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_emailConfiguration.SmtpServer, 465, SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

    }

    public class EmailConfiguration
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string From { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
    }

    public class EmailMessage
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }


}
