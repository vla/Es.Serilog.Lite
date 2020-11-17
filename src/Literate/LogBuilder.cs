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
        public static ILogger Create(Action<LoggerSinkConfiguration> configure = null)
        {
            return Create(new SerilogOptions
            {
                LogMinLevel = LogEventLevel.Debug,
                StdOut = true
            }, configure);
        }

        /// <summary>
        /// Build the Serilog instance.
        /// </summary>
        /// <param name="serilogOptions"><see cref="SerilogOptions" /></param>
        /// <param name="environmentName">Development|Production|Staging</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static ILogger Create(SerilogOptions serilogOptions, string environmentName, Action<LoggerSinkConfiguration> configure = null)
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

            return Create(serilogOptions, configure);
        }

        /// <summary>
        /// Build the Serilog instance.
        /// </summary>
        /// <param name="serilogOptions"><see cref="SerilogOptions" /></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static ILogger Create(SerilogOptions serilogOptions, Action<LoggerSinkConfiguration> configure = null)
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

            if (serilogOptions.Async)
            {
                configuration.WriteTo.Async(async =>
                {
                    Configue(async, serilogOptions, configure);
                });
            }
            else
            {
                Configue(configuration.WriteTo, serilogOptions, configure);
            }

            if (serilogOptions.SourceContextFilterOptions != null)
            {
                configuration.ConfigueSourceContextFilter(serilogOptions.SourceContextFilterOptions);
            }

            return configuration.CreateLogger();
        }

        private static void Configue(LoggerSinkConfiguration configuration, SerilogOptions serilogOptions, Action<LoggerSinkConfiguration> configure = null)
        {
            var outputTemplate = serilogOptions.OutputTemplate ?? SerilogExtensions.DefaultOutputTemplate;

            JsonFormatter jsonFormatter = null;

            if (serilogOptions.FormatJson)
            {
                jsonFormatter = new JsonFormatter();
            }

            var obsoletePathFormat = serilogOptions.PathFormat;
            var path = serilogOptions.Path;

            //compatibility
            if (!string.IsNullOrWhiteSpace(obsoletePathFormat) && string.IsNullOrWhiteSpace(path))
            {
                path = obsoletePathFormat;
            }

            if (!string.IsNullOrWhiteSpace(path))
            {
                // whether relative path is used, the root directory is applied by default.
                if (!Path.IsPathRooted(path))
                {
#if NETFULL
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
#else
                    path = Path.Combine(AppContext.BaseDirectory, path);
#endif
                }
            }
            else
            {
                //By default, the 'logs' directory of the root directory.
#if NETFULL
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "log.log");
#else
                path = Path.Combine(AppContext.BaseDirectory, "logs", "log.log");
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
                    configuration.ConfigueFile(jsonFormatter, path: path);
                }
                else
                {
                    configuration.ConfigueFile(path: path, outputTemplate: outputTemplate);
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

            configure?.Invoke(configuration);
        }
    }
}