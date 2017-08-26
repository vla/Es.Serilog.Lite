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

#if NETSTANDARD2_0 || NET45
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;

namespace Es.Serilog.Lite.Email
{
    class EmailSink : PeriodicBatchingSink
    {
        readonly EmailConfig _config;
        readonly SmtpClient _smtpClient;
        readonly ITextFormatter _textFormatter;
        readonly ITextFormatter _subjectLineFormatter;

        /// <summary>
        /// A reasonable default for the number of events posted in
        /// each batch.
        /// </summary>
        public const int DefaultBatchPostingLimit = 100;

        /// <summary>
        /// A reasonable default time to wait between checking for event batches.
        /// </summary>
        public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(30);

        public EmailSink(
            EmailConfig config,
            int batchSizeLimit, TimeSpan period,
            ITextFormatter textFormatter,
            ITextFormatter subjectLineFormatter)
            : base(batchSizeLimit, period)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _textFormatter = textFormatter;
            _subjectLineFormatter = subjectLineFormatter;
            _smtpClient = CreateSmtpClient();
            _smtpClient.SendCompleted += SendCompletedCallback;
        }

        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
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

        protected async override Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            var payload = new StringWriter();

            foreach (var logEvent in events)
            {
                _textFormatter.Format(logEvent, payload);
            }

            var subject = new StringWriter();
            _subjectLineFormatter.Format(events.OrderByDescending(e => e.Level).First(), subject);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config.FromEmail),
                Subject = subject.ToString(),
                Body = payload.ToString(),
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
                IsBodyHtml = _config.IsBodyHtml
            };

            foreach (var recipient in _config.ToEmail.Split(",;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                mailMessage.To.Add(recipient);
            }

            await _smtpClient.SendMailAsync(mailMessage);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                _smtpClient.Dispose();
        }

        private SmtpClient CreateSmtpClient()
        {
            var smtpClient = new SmtpClient();
            if (!string.IsNullOrWhiteSpace(_config.MailServer))
            {
                if (_config.NetworkCredentials == null)
                    smtpClient.UseDefaultCredentials = true;
                else
                    smtpClient.Credentials = _config.NetworkCredentials;

                smtpClient.Host = _config.MailServer;
                smtpClient.Port = _config.Port;
                smtpClient.EnableSsl = _config.EnableSsl;
            }

            return smtpClient;
        }
    }
}

#endif