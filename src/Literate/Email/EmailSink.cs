// Copyright 2014 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;

namespace Es.Serilog.Lite.Email
{
    internal class EmailSink : PeriodicBatchingSink
    {
        private readonly EmailConfig _emailConfig;

        private readonly MimeKit.InternetAddress _fromAddress;
        private readonly IEnumerable<MimeKit.InternetAddress> _toAddresses;

        private readonly ITextFormatter _textFormatter;

        private readonly ITextFormatter _subjectFormatter;

        /// <summary>
        /// A reasonable default for the number of events posted in
        /// each batch.
        /// </summary>
        public const int DefaultBatchPostingLimit = 100;

        /// <summary>
        /// A reasonable default time to wait between checking for event batches.
        /// </summary>
        public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Construct a sink emailing with the specified details.
        /// </summary>
        /// <param name="emailConfig">Connection information used to construct the SMTP client and mail messages.</param>
        /// <param name="batchSizeLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="textFormatter">Supplies culture-specific formatting information, or null.</param>
        /// <param name="subjectLineFormatter">The subject line formatter.</param>
        /// <exception cref="System.ArgumentNullException">connectionInfo</exception>
        public EmailSink(EmailConfig emailConfig, int batchSizeLimit, TimeSpan period, ITextFormatter textFormatter, ITextFormatter subjectLineFormatter)
            : base(batchSizeLimit, period)
        {
            _emailConfig = emailConfig ?? throw new ArgumentNullException(nameof(emailConfig));

            _fromAddress = MimeKit.MailboxAddress.Parse(_emailConfig.FromEmail);
            _toAddresses = emailConfig
                .ToEmail
                .Split(",;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(MimeKit.MailboxAddress.Parse)
                .ToArray();

            _textFormatter = textFormatter;
            _subjectFormatter = subjectLineFormatter;
        }

        private MimeKit.MimeMessage CreateMailMessage(string payload, string subject)
        {
            var mailMessage = new MimeKit.MimeMessage();
            mailMessage.From.Add(_fromAddress);
            mailMessage.To.AddRange(_toAddresses);
            mailMessage.Subject = subject;
            mailMessage.Body = _emailConfig.IsBodyHtml
                ? new MimeKit.BodyBuilder { HtmlBody = payload }.ToMessageBody()
                : new MimeKit.BodyBuilder { TextBody = payload }.ToMessageBody();
            return mailMessage;
        }

        /// <summary>
        /// Emit a batch of log events, running asynchronously.
        /// </summary>
        /// <param name="events">The events to emit.</param>
        /// <remarks>Override either <see cref="PeriodicBatchingSink.EmitBatch"/> or <see cref="PeriodicBatchingSink.EmitBatchAsync"/>,
        /// not both.</remarks>
        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            var payload = new StringWriter();

            foreach (var logEvent in events)
            {
                _textFormatter.Format(logEvent, payload);
            }

            var subject = new StringWriter();
            _subjectFormatter.Format(events.OrderByDescending(e => e.Level).First(), subject);

            var mailMessage = CreateMailMessage(payload.ToString(), subject.ToString());

            try
            {
                using (var smtpClient = OpenConnectedSmtpClient())
                {
                    await smtpClient.SendAsync(mailMessage);
                    await smtpClient.DisconnectAsync(quit: true);
                }
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine("Failed to send email: {0}", ex.ToString());
            }
        }

        private SmtpClient OpenConnectedSmtpClient()
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

                if (_emailConfig.Timeout > 0)
                    smtpClient.Timeout = _emailConfig.Timeout;

                smtpClient.Connect(_emailConfig.MailServer, _emailConfig.Port, SecureSocketOptions.Auto);

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

        private static bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}