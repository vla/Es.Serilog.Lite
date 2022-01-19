using System;
using System.Threading.Tasks;

namespace Es.Serilog.Lite.Email
{
    internal interface IEmailTransport : IDisposable
    {
        Task SendMailAsync(EmailMessage emailMessage);
    }
}