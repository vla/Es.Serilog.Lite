using System;
using System.IO;
using Es.Serilog.Lite;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.SystemConsole.Themes;

namespace Serilog
{
    /// <summary>
    /// Serilog Extensions
    /// </summary>
    public static class SerilogExtensions
    {
        /// <summary>
        /// Default OutputTemplate
        /// </summary>
        public const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:w} {SourceContext} {Message}{NewLine}{Exception}";

        /// <summary>
        /// Configue Default
        /// </summary>
        /// <param name="configuration"><see cref="LoggerConfiguration"/></param>
        /// <returns><see cref="LoggerConfiguration"/></returns>
        public static LoggerConfiguration Configue(this LoggerConfiguration configuration)
        {
            return configuration
                .Enrich.FromLogContext();
        }

        /// <summary>
        /// Configues the source context filter.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static LoggerConfiguration ConfigueSourceContextFilter(this LoggerConfiguration configuration, SourceContextFilterOptions options)
        {
            return configuration.Filter.With(new SourceContextFilter(options));
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
#if NETFULL
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
#if NETFULL
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
        /// <param name="minLevel"><see cref="LogEventLevel"/></param>
        /// <returns><see cref="LoggerConfiguration"/></returns>
        public static LoggerConfiguration ConfigueLevel(this LoggerConfiguration configuration, LogEventLevel minLevel = LevelAlias.Minimum)
        {
            switch (minLevel)
            {
                case LogEventLevel.Fatal:
                    return configuration.MinimumLevel.Fatal();

                case LogEventLevel.Error:
                    return configuration.MinimumLevel.Error();

                case LogEventLevel.Warning:
                    return configuration.MinimumLevel.Warning();

                case LogEventLevel.Information:
                    return configuration.MinimumLevel.Information();

                case LogEventLevel.Debug:
                    return configuration.MinimumLevel.Debug();

                default:
                    return configuration.MinimumLevel.Verbose();
            }
        }
    }
}