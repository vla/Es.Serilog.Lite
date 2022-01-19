using Serilog.Debugging;
using System.ComponentModel;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Es.Serilog.Lite.Email
{
    internal class SystemMailEmailTransport : IEmailTransport
    {
        private readonly SmtpClient _smtpClient;

        public SystemMailEmailTransport(EmailConfig emailConfig)
        {
            _smtpClient = CreateSmtpClient(emailConfig);
            _smtpClient.SendCompleted += SendCompletedCallback;
        }

        public async Task SendMailAsync(EmailMessage emailMessage)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailMessage.From),
                Subject = emailMessage.Subject,
                Body = emailMessage.Body,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
                IsBodyHtml = emailMessage.IsBodyHtml
            };

            foreach (var recipient in emailMessage.To)
            {
                mailMessage.To.Add(recipient);
            }

            await _smtpClient.SendMailAsync(mailMessage);
        }

        public void Dispose()
        {
            _smtpClient.Dispose();
        }

        private SmtpClient CreateSmtpClient(EmailConfig emailConfig)
        {
            var smtpClient = new SmtpClient();
            if (!string.IsNullOrWhiteSpace(emailConfig.MailServer))
            {
                if (emailConfig.NetworkCredentials == null)
                {
                    smtpClient.UseDefaultCredentials = true;
                }
                else
                {
                    smtpClient.Credentials = emailConfig.NetworkCredentials;
                }

                smtpClient.Host = emailConfig.MailServer;
                smtpClient.Port = emailConfig.Port;
                smtpClient.EnableSsl = emailConfig.EnableSsl;
            }

            return smtpClient;
        }

        /// <summary>
        ///     Reports if there is an error in sending an email
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                SelfLog.WriteLine("Received failed result {0}: {1}", "Cancelled", e.Error);
            }

            if (e.Error != null)
            {
                SelfLog.WriteLine("Received failed result {0}: {1}", "Error", e.Error);
            }
        }
    }
}