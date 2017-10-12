using System;
using System.IO;
using Es.Serilog.Lite.Enricher;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.SystemConsole.Themes;

namespace Serilog
{
    /// <summary>
    /// Serilog Extensions
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Default OutputTemplate
        /// </summary>
        public const string DefaultOutputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss}] ({MachineName}) {Level:w} {Logger} {Message}{NewLine}{Exception}";

        private const string DefaultFilter = "\"Microsoft";

        /// <summary>
        /// Configue All
        /// </summary>
        /// <param name="configuration"><see cref="LoggerConfiguration"/></param>
        /// <param name="environmentName">Production|Staging|Development</param>
        /// <param name="skipMicrosoftLog">Skip Microsoft logs and so log only own logs</param>
        /// <returns><see cref="LoggerConfiguration"/></returns>
        public static LoggerConfiguration ConfigueAll(this LoggerConfiguration configuration,
            string environmentName = "Production",
            bool skipMicrosoftLog = true)
        {
            configuration = configuration
                .Configue()
                .ConfigueLevel(environmentName)
                .WriteTo.Async(async =>
                {
                    async.ConfigueStd();
                    async.ConfigueRollingFile();
                });
            if (skipMicrosoftLog)
                return configuration.ConfigueSkipMicrosoftLog();
            return configuration;
        }

        /// <summary>
        /// Configue Default
        /// </summary>
        /// <param name="configuration"><see cref="LoggerConfiguration"/></param>
        /// <returns><see cref="LoggerConfiguration"/></returns>
        public static LoggerConfiguration Configue(this LoggerConfiguration configuration)
        {
            return configuration
                .Enrich.WithMachineName()
                .Enrich.FromLogContext()
                .Enrich.With<LoggerNameEnricher>();
        }

        /// <summary>
        /// Configue Skip Microsoft logs and so log only own logs
        /// </summary>
        /// <param name="configuration"><see cref="LoggerConfiguration"/></param>
        /// <returns><see cref="LoggerConfiguration"/></returns>
        public static LoggerConfiguration ConfigueSkipMicrosoftLog(this LoggerConfiguration configuration)
        {
            return configuration.Filter.ByExcluding(f =>
                f.Properties.ContainsKey(Constants.SourceContextPropertyName)
                && f.Properties.TryGetValue(Constants.SourceContextPropertyName, out LogEventPropertyValue v)
                && v.ToString().StartsWith(DefaultFilter)
            );
        }

        /// <summary>
        /// Configure standard input/output
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="outputTemplate"></param>
        /// <param name="theme"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="formatProvider"></param>
        /// <param name="stdErrorFromLevel"></param>
        /// <returns><see cref="LoggerConfiguration"/></returns>
        public static LoggerConfiguration ConfigueStd(this LoggerSinkConfiguration configuration,
            string outputTemplate = DefaultOutputTemplate,
             ConsoleTheme theme = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider formatProvider = null,
            LogEventLevel? stdErrorFromLevel = null
           )
        {
            return configuration.Console(
                outputTemplate: outputTemplate,
                formatProvider: formatProvider,
                standardErrorFromLevel: stdErrorFromLevel,
                theme: theme ?? SystemConsoleTheme.Literate,
                restrictedToMinimumLevel: restrictedToMinimumLevel);
        }

        /// <summary>
        /// Configure standard input/output
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="formatter"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="stdErrorFromLevel"></param>
        /// <returns><see cref="LoggerConfiguration"/></returns>
        public static LoggerConfiguration ConfigueStd(this LoggerSinkConfiguration configuration,
            ITextFormatter formatter,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            LogEventLevel? stdErrorFromLevel = null
           )
        {
            return configuration.Console(
                formatter,
                restrictedToMinimumLevel,
                null,
                stdErrorFromLevel);
        }

        /// <summary>
        /// Configure RollingFile
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="outputTemplate"></param>
        /// <param name="shared"></param>
        /// <param name="buffered"></param>
        /// <param name="retainedFileCountLimit"></param>
        /// <param name="fileSizeLimitBytes"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="formatProvider"></param>
        /// <param name="pathFormat"></param>
        /// <returns><see cref="LoggerConfiguration"/></returns>
        public static LoggerConfiguration ConfigueRollingFile(this LoggerSinkConfiguration configuration,
            string outputTemplate = DefaultOutputTemplate,
            bool shared = true,
            bool buffered = false,
            int retainedFileCountLimit = 31,
            int fileSizeLimitBytes = 1073741824,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider formatProvider = null,
            string pathFormat = null)
        {
            return configuration.RollingFile(
                pathFormat: pathFormat ?? Path.Combine(
#if NET45
                    AppDomain.CurrentDomain.BaseDirectory
#else
                    AppContext.BaseDirectory
#endif
                    , "logs", "{Date}.log"),
                shared: shared,
                buffered: buffered,
                restrictedToMinimumLevel: restrictedToMinimumLevel,
                formatProvider: formatProvider,
                retainedFileCountLimit: retainedFileCountLimit,
                fileSizeLimitBytes: fileSizeLimitBytes,
                outputTemplate: outputTemplate);
        }

        /// <summary>
        /// Configure RollingFile
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="formatter"></param>
        /// <param name="shared"></param>
        /// <param name="buffered"></param>
        /// <param name="retainedFileCountLimit"></param>
        /// <param name="fileSizeLimitBytes"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="formatProvider"></param>
        /// <param name="pathFormat"></param>
        /// <returns><see cref="LoggerConfiguration"/></returns>
        public static LoggerConfiguration ConfigueRollingFile(this LoggerSinkConfiguration configuration,
            ITextFormatter formatter,
            bool shared = true,
            bool buffered = false,
            int retainedFileCountLimit = 31,
            int fileSizeLimitBytes = 1073741824,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider formatProvider = null,
            string pathFormat = null)
        {
            return configuration.RollingFile(formatter,
                                pathFormat ?? Path.Combine(
#if NET45
                    AppDomain.CurrentDomain.BaseDirectory
#else
                    AppContext.BaseDirectory
#endif
                    , "logs", "{Date}.log"),
                    restrictedToMinimumLevel,
                    fileSizeLimitBytes,
                    retainedFileCountLimit,
                    shared: shared,
                    buffered: buffered

                );
        }

        /// <summary>
        /// Configuration log level
        /// </summary>
        /// <param name="configuration"><see cref="LoggerConfiguration"/></param>
        /// <param name="environmentName">Production|Staging|Development</param>
        /// <returns><see cref="LoggerConfiguration"/></returns>
        public static LoggerConfiguration ConfigueLevel(this LoggerConfiguration configuration, string environmentName = "Production")
        {
            if (environmentName.Equals("Production", StringComparison.OrdinalIgnoreCase))
            {
                return configuration.MinimumLevel.Information();
            }
            else if (environmentName.Equals("Development", StringComparison.OrdinalIgnoreCase))
            {
                return configuration.MinimumLevel.Verbose();
            }
            else
            {
                //Staging
                return configuration.MinimumLevel.Debug();
            }
        }
    }
}