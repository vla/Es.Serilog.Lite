using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;


namespace Es.Serilog.Lite.Email
{
    class MailKitEmailTransport : IEmailTransport
    {
        readonly EmailConfig _emailConfig;

        public MailKitEmailTransport(EmailConfig  emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public async Task SendMailAsync(EmailMessage emailMessage)
        {
            var fromAddress = MailboxAddress.Parse(emailMessage.From);
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(fromAddress);
            mimeMessage.To.AddRange(emailMessage.To.Select(MailboxAddress.Parse));
            mimeMessage.Subject = emailMessage.Subject;
            mimeMessage.Body = _emailConfig.IsBodyHtml
                ? new BodyBuilder { HtmlBody = emailMessage.Body }.ToMessageBody()
                : new BodyBuilder { TextBody = emailMessage.Body }.ToMessageBody();
            using (var smtpClient = OpenConnectedSmtpClient())
            {
                await smtpClient.SendAsync(mimeMessage);
                await smtpClient.DisconnectAsync(quit: true);
            }
        }
        SmtpClient OpenConnectedSmtpClient()
        {
            var smtpClient = new SmtpClient();
            if (!string.IsNullOrWhiteSpace(_emailConfig.MailServer))
            {
                if (_emailConfig.ServerCertificateValidationCallback != null)
                {
                    smtpClient.ServerCertificateValidationCallback += _emailConfig.ServerCertificateValidationCallback;
                }
                else
                {
                    smtpClient.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
                }

                smtpClient.Connect(
                    _emailConfig.MailServer, _emailConfig.Port,
                    useSsl: _emailConfig.EnableSsl);

                if (_emailConfig.NetworkCredentials != null)
                {
                    smtpClient.Authenticate(
                        Encoding.UTF8,
                        _emailConfig.NetworkCredentials.GetCredential(
                            _emailConfig.MailServer, _emailConfig.Port, "smtp"));
                }
            }
            return smtpClient;
        }

        public void Dispose()
        {
        }

        private static bool ServerCertificateValidationCallback(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
