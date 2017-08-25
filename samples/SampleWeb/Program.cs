using System.IO;
using System.Reflection;
using Es.Serilog.Lite;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace SampleWeb
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var builder = new WebHostBuilder()
                .UseKestrel(x =>
                {
                    x.AddServerHeader = false;
                })
                 .UseUrls("Http://*:5000")
                 .UseContentRoot(Directory.GetCurrentDirectory())
                 .ConfigureAppConfiguration((hostingContext, config) =>
                 {
                     var env = hostingContext.HostingEnvironment;

                     config.SetBasePath(env.ContentRootPath)
                      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                     if (env.IsDevelopment())
                     {
                         var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                         if (appAssembly != null)
                         {
                             config.AddUserSecrets(appAssembly, optional: true);
                         }

                         config.AddApplicationInsightsSettings(developerMode: true);
                     }

                     config.AddEnvironmentVariables();

                     if (args != null)
                     {
                         config.AddCommandLine(args);
                     }
                 })
                 .ConfigureLogging((hostingContext, logging) =>
                 {
                     logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));

                     Log.Logger = new LoggerConfiguration()
                     .ConfigueAll(hostingContext.HostingEnvironment.EnvironmentName)
                     .CreateLogger();

                     Log.Information("Getting the motors running...");
                     logging.AddSerilog(logger: Log.Logger, dispose: true);
                 })
                 .UseIISIntegration()
                 .UseDefaultServiceProvider((context, options) =>
                 {
                     options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                 })
                 .UseApplicationInsights()
                 .UseStartup<Startup>();

            return builder.Build();
        }
    }
}