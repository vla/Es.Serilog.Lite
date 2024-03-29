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

using MailKit.Security;
using System.Net;

namespace Es.Serilog.Lite.Email
{
    /// <summary>
    /// Connection information for use by the Email sink.
    /// </summary>
    public class EmailConfig
    {
        /// <summary>
        /// The default port used by for SMTP transfer.
        /// </summary>
        private const int DefaultPort = 25;

        /// <summary>
        /// The default subject used for email messages.
        /// </summary>
        public const string DefaultSubject = "Log Email";

        /// <summary>
        /// Constructs the <see cref="EmailConfig"/> with the default port and default email subject set.
        /// </summary>
        public EmailConfig()
        {
            Port = DefaultPort;
            EmailSubject = DefaultSubject;
            IsBodyHtml = false;
        }

        /// <summary>
        /// Gets or sets the credentials used for authentication.
        /// </summary>
        public ICredentialsByHost? NetworkCredentials { get; set; }


        /// <summary>
        ///Get or set the timeout for network streaming operations, in milliseconds.
        /// </summary>
        public int Timeout { get; set; } = 5000;

        /// <summary>
        /// Gets or sets the port used for the connection.
        /// Default value is 25.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The email address emails will be sent from.
        /// </summary>
        public string FromEmail { get; set; } = default!;

        /// <summary>
        /// The email address(es) emails will be sent to. Accepts multiple email addresses separated by comma or semicolon.
        /// </summary>
        public string ToEmail { get; set; } = default!;

        /// <summary>
        /// The subject to use for the email, this can be a template.
        /// </summary>
        public string EmailSubject { get; set; } = default!;

        /// <summary>
        /// Flag as true to use SSL in the SMTP client.
        /// </summary>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// Provides a method that validates server certificates.
        /// </summary>
        public System.Net.Security.RemoteCertificateValidationCallback? ServerCertificateValidationCallback { get; set; }

        /// <summary>
        /// The SMTP email server to use.
        /// </summary>
        public string? MailServer { get; set; }

        /// <summary>
        /// Sets whether the body contents of the email is HTML. Defaults to false.
        /// </summary>
        public bool IsBodyHtml { get; set; }

        /// <summary>
        /// The user name associated with the credentials.
        /// </summary>
        public string? Account { get; set; }

        /// <summary>
        /// he password for the user name associated with the credentials.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Set secure socket option
        /// </summary>
        public SecureSocketOptions? SecureSocketOption { get; set; }


        internal virtual IEmailTransport CreateEmailTransport()
        {
            return new MailKitEmailTransport(this);
        }


    }
}