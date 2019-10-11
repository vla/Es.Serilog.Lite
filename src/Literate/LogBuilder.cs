using System;
using System.IO;
using System.Net;
using Es.Serilog.Lite.Formatting;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace Es.Serilog.Lite
{
    /// <summary>
    /// LogBuilder
    /// </summary>
    public class LogBuilder
    {
        /// <summary>
        /// Build the StdOut Serilog instance, LogLevel is Debug
        /// </summary>
        /// <returns></returns>
        public static ILogger Create()
        {
            return Create(new SerilogOptions
            {
                LogMinLevel = LogEventLevel.Debug,
                StdOut = true
            });
        }

        /// <summary>
        /// Build the Serilog instance.
        /// </summary>
        /// <param name="serilogOptions"><see cref="SerilogOptions" /></param>
        /// <param name="environmentName">Development|Production|Staging</param>
        /// <returns></returns>
        public static ILogger Create(SerilogOptions serilogOptions, string environmentName)
        {
            if (serilogOptions == null)
            {
                serilogOptions = new SerilogOptions();
            }

            if (!serilogOptions.LogMinLevel.HasValue)
            {
                switch (environmentName.ToLower())
                {
                    case "production":
                        serilogOptions.LogMinLevel = LogEventLevel.Information;
                        break;
                    case "staging":
                        serilogOptions.LogMinLevel = LogEventLevel.Debug;
                        break;
                    default:
                        serilogOptions.LogMinLevel = LogEventLevel.Verbose;
                        break;
                }
            }

            return Create(serilogOptions);
        }

        /// <summary>
        /// Build the Serilog instance.
        /// </summary>
        /// <param name="serilogOptions"><see cref="SerilogOptions" /></param>
        /// <returns></returns>
        public static ILogger Create(SerilogOptions serilogOptions)
        {
            if (serilogOptions == null)
            {
                serilogOptions = new SerilogOptions();
            }

            var configuration = new LoggerConfiguration().Configue();

            if (!serilogOptions.LogMinLevel.HasValue)
            {
                serilogOptions.LogMinLevel = LogEventLevel.Debug;
            }

            configuration.ConfigueLevel(serilogOptions.LogMinLevel.Value);

            var sinkConfig = configuration.WriteTo;

            if (serilogOptions.Async)
            {
                configuration.WriteTo.Async(async =>
                {
                    Configue(async, serilogOptions);
                });
            }
            else
            {
                Configue(configuration.WriteTo, serilogOptions);
            }

            if (serilogOptions.SourceContextFilterOptions != null)
            {
                configuration.ConfigueSourceContextFilter(serilogOptions.SourceContextFilterOptions);
            }

            return configuration.CreateLogger();
        }

        private static void Configue(LoggerSinkConfiguration configuration, SerilogOptions serilogOptions)
        {
            var outputTemplate = serilogOptions.OutputTemplate ?? SerilogExtensions.DefaultOutputTemplate;

            JsonFormatter jsonFormatter = null;

            if (serilogOptions.FormatJson)
            {
                jsonFormatter = new JsonFormatter();
            }

            var pathFormat = serilogOptions.PathFormat;

            if (!string.IsNullOrWhiteSpace(pathFormat))
            {
                // whether relative path is used, the root directory is applied by default.
                if (!Path.IsPathRooted(pathFormat))
                {
#if NETFULL
                    pathFormat = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pathFormat);
#else
                    pathFormat = Path.Combine(AppContext.BaseDirectory, pathFormat);
#endif
                }
            }
            else
            {
                //By default, the 'logs' directory of the root directory.
#if NETFULL
                pathFormat = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "{Date}.log");
#else
                pathFormat = Path.Combine(AppContext.BaseDirectory, "logs", "{Date}.log");
#endif
            }

            //stdout
            if (serilogOptions.StdOut)
            {
                if (serilogOptions.FormatJson)
                {
                    configuration.ConfigueStd(jsonFormatter);
                }
                else
                {
                    configuration.ConfigueStd(outputTemplate: outputTemplate);
                }
            }

            //file Record
            if (serilogOptions.RollingFile)
            {
                if (serilogOptions.FormatJson)
                {
                    configuration.ConfigueRollingFile(jsonFormatter, pathFormat: pathFormat);
                }
                else
                {
                    configuration.ConfigueRollingFile(pathFormat: pathFormat, outputTemplate: outputTemplate);
                }
            }

            //email
            if (serilogOptions.Email != null)
            {
                if (!string.IsNullOrWhiteSpace(serilogOptions.Email.Account) && !string.IsNullOrWhiteSpace(serilogOptions.Email.Password))
                {
                    serilogOptions.Email.NetworkCredentials = new NetworkCredential(serilogOptions.Email.Account, serilogOptions.Email.Password);
                }

                configuration.Email(serilogOptions.Email, outputTemplate: outputTemplate, restrictedToMinimumLevel: LogEventLevel.Warning);
            }
        }
    }
}