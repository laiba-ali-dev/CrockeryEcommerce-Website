using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace ecommerce.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // Read SMTP settings from appsettings.json
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(smtpSettings["SenderName"], smtpSettings["SenderEmail"]));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = subject;

            // Email Body
            email.Body = new TextPart("html")
            {
                Text = body
            };

            using (var smtpClient = new SmtpClient())
            {
                try
                {
                    await smtpClient.ConnectAsync(smtpSettings["Server"], int.Parse(smtpSettings["Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                    await smtpClient.AuthenticateAsync(smtpSettings["SenderEmail"], smtpSettings["Password"]);
                    await smtpClient.SendAsync(email);
                }
                finally
                {
                    await smtpClient.DisconnectAsync(true);
                }
            }
        }

    }
}
