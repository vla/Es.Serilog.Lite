﻿// Copyright 2014 Serilog Contributors
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
using System.Net;
using Es.Serilog.Lite.Email;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog
{
    /// <summary>
    /// Adds the WriteTo.Email() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class LoggerConfigurationEmailExtensions
    {
        const int DefaultBatchPostingLimit = 100;
        static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Adds a sink that sends log events via email.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="fromEmail">The email address emails will be sent from</param>
        /// <param name="toEmail">The email address emails will be sent to</param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink.
        /// the default is "{Timestamp} [{Level}] {Message}{NewLine}{Exception}".</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="mailSubject">The subject, can be a plain string or a template such as {Timestamp} [{Level}] occurred.</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration Email(
            this LoggerSinkConfiguration loggerConfiguration,
            string fromEmail,
            string toEmail,
            string outputTemplate = SerilogExtensions.DefaultOutputTemplate,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = DefaultBatchPostingLimit,
            TimeSpan? period = null,
            IFormatProvider? formatProvider = null,
            string mailSubject = EmailConfig.DefaultSubject)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException("loggerConfiguration");
            if (fromEmail == null) throw new ArgumentNullException("fromEmail");
            if (toEmail == null) throw new ArgumentNullException("toEmail");

            var emailConfig = new EmailConfig
            {
                FromEmail = fromEmail,
                ToEmail = toEmail,
                EmailSubject = mailSubject
            };

            return Email(loggerConfiguration, emailConfig, outputTemplate, restrictedToMinimumLevel, batchPostingLimit, period, formatProvider);
        }

        /// <summary>
        /// Adds a sink that sends log events via email.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="fromEmail">The email address emails will be sent from</param>
        /// <param name="toEmail">The email address emails will be sent to</param>
        /// <param name="mailServer">The SMTP email server to use</param>
        /// <param name="networkCredential">The network credentials to use to authenticate with mailServer</param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink.
        /// the default is "{Timestamp} [{Level}] {Message}{NewLine}{Exception}".</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="mailSubject">The subject, can be a plain string or a template such as {Timestamp} [{Level}] occurred.</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration Email(
            this LoggerSinkConfiguration loggerConfiguration,
            string fromEmail,
            string toEmail,
            string mailServer,
            ICredentialsByHost? networkCredential = null,
            string outputTemplate = SerilogExtensions.DefaultOutputTemplate,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = DefaultBatchPostingLimit,
            TimeSpan? period = null,
            IFormatProvider? formatProvider = null,
            string mailSubject = EmailConfig.DefaultSubject)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException("loggerConfiguration");
            if (fromEmail == null) throw new ArgumentNullException("fromEmail");
            if (toEmail == null) throw new ArgumentNullException("toEmail");

            var emailConfig = new EmailConfig
            {
                FromEmail = fromEmail,
                ToEmail = toEmail,
                MailServer = mailServer,
                NetworkCredentials = networkCredential,
                EmailSubject = mailSubject
            };

            return Email(loggerConfiguration, emailConfig, outputTemplate, restrictedToMinimumLevel, batchPostingLimit, period, formatProvider);
        }

        /// <summary>
        /// Adds a sink that sends log events via email.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="fromEmail">The email address emails will be sent from</param>
        /// <param name="toEmails">The email addresses emails will be sent to</param>
        /// <param name="mailServer">The SMTP email server to use</param>
        /// <param name="networkCredential">The network credentials to use to authenticate with mailServer</param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink.
        /// the default is "{Timestamp} [{Level}] {Message}{NewLine}{Exception}".</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="mailSubject">The subject, can be a plain string or a template such as {Timestamp} [{Level}] occurred.</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration Email(
            this LoggerSinkConfiguration loggerConfiguration,
            string fromEmail,
            IEnumerable<string> toEmails,
            string mailServer,
            ICredentialsByHost? networkCredential = null,
            string outputTemplate = SerilogExtensions.DefaultOutputTemplate,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = DefaultBatchPostingLimit,
            TimeSpan? period = null,
            IFormatProvider? formatProvider = null,
            string mailSubject = EmailConfig.DefaultSubject)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException("loggerConfiguration");
            if (fromEmail == null) throw new ArgumentNullException("fromEmail");
            if (toEmails == null) throw new ArgumentNullException("toEmails");

            var emailConfig = new EmailConfig
            {
                FromEmail = fromEmail,
                ToEmail = string.Join(";", toEmails),
                MailServer = mailServer,
                NetworkCredentials = networkCredential,
                EmailSubject = mailSubject
            };

            return Email(loggerConfiguration, emailConfig, outputTemplate, restrictedToMinimumLevel, batchPostingLimit, period, formatProvider, mailSubject);
        }

        /// <summary>
        /// Adds a sink that sends log events via email.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="emailConfig">The connection info used for </param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink.
        /// the default is "{Timestamp} [{Level}] {Message}{NewLine}{Exception}".</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="mailSubject">The subject, can be a plain string or a template such as {Timestamp} [{Level}] occurred.</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration Email(
            this LoggerSinkConfiguration loggerConfiguration,
            EmailConfig emailConfig,
            string outputTemplate = SerilogExtensions.DefaultOutputTemplate,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = DefaultBatchPostingLimit,
            TimeSpan? period = null,
            IFormatProvider? formatProvider = null,
            string mailSubject = EmailConfig.DefaultSubject)
        {
            if (emailConfig == null) throw new ArgumentNullException("emailConfig");

            if (!string.IsNullOrEmpty(emailConfig.EmailSubject))
            {
                mailSubject = emailConfig.EmailSubject;
            }

            var batchingPeriod = period ?? DefaultPeriod;
            var textFormatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            var mailSubjectFormatter = new MessageTemplateTextFormatter(mailSubject, formatProvider);

            var batchingOptions = new PeriodicBatchingSinkOptions
            {
                BatchSizeLimit = batchPostingLimit,
                Period = batchingPeriod,
                EagerlyEmitFirstEvent = false,  // set default to false, not usable for emailing
                QueueLimit = 10000
            };
            var batchingSink = new PeriodicBatchingSink(new EmailSink(emailConfig, textFormatter, mailSubjectFormatter), batchingOptions);

            return loggerConfiguration.Sink(batchingSink, restrictedToMinimumLevel);
        }

        /// <summary>
        /// Adds a sink that sends log events via email.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="emailConfig">The connection info used for </param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="textFormatter">ITextFormatter implementation to write log entry to email.</param>
        /// <param name="mailSubject">The subject, can be a plain string or a template such as {Timestamp} [{Level}] occurred.</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration Email(
            this LoggerSinkConfiguration loggerConfiguration,
            EmailConfig emailConfig,
            ITextFormatter textFormatter,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = DefaultBatchPostingLimit,
            TimeSpan? period = null,
            string mailSubject = EmailConfig.DefaultSubject)
        {
            if (emailConfig == null) throw new ArgumentNullException("emailConfig");
            if (textFormatter == null) throw new ArgumentNullException("textFormatter");

            ITextFormatter mailSubjectFormatter = new MessageTemplateTextFormatter(mailSubject, null);

            var batchingPeriod = period ?? DefaultPeriod;

            var batchingOptions = new PeriodicBatchingSinkOptions
            {
                BatchSizeLimit = batchPostingLimit,
                Period = batchingPeriod,
                EagerlyEmitFirstEvent = false,  // set default to false, not usable for emailing
                QueueLimit = 10000
            };
            var batchingSink = new PeriodicBatchingSink(new EmailSink(emailConfig, textFormatter, mailSubjectFormatter), batchingOptions);

            return loggerConfiguration.Sink(batchingSink, restrictedToMinimumLevel);
        }
    }
}