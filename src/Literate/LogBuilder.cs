using System;
using System.IO;
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
                LogMinLevel = "Debug",
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

            if (string.IsNullOrWhiteSpace(serilogOptions.LogMinLevel))
            {
                switch (environmentName)
                {
                    case "Production":
                        serilogOptions.LogMinLevel = "Information";
                        break;
                    case "Staging":
                        serilogOptions.LogMinLevel = "Debug";
                        break;
                    default:
                        serilogOptions.LogMinLevel = "Verbose";
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

            var defaultLevel = LogEventLevel.Verbose;

            if (!string.IsNullOrWhiteSpace(serilogOptions.LogMinLevel))
            {
                if (!Enum.TryParse(serilogOptions.LogMinLevel, out defaultLevel))
                {
                    defaultLevel = LogEventLevel.Verbose;
                }
            }

            configuration.ConfigueLevel(defaultLevel);

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

            //Filter Microsoft log information.
            if (serilogOptions.SkipMicrosoftLog)
            {
                configuration.ConfigueSkipMicrosoftLog();
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
                configuration.Email(serilogOptions.Email, outputTemplate: outputTemplate, restrictedToMinimumLevel: LogEventLevel.Warning);
            }
        }
    }
}