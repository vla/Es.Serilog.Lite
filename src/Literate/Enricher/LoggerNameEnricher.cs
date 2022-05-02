using Serilog.Core;
using Serilog.Events;

namespace Es.Serilog.Lite.Enricher
{
    /// <summary>
    /// LoggerNameEnricher
    /// </summary>
    public class LoggerNameEnricher : ILogEventEnricher
    {
        /// <summary>
        /// Logger PropertyName
        /// </summary>
        public const string LoggerNamePropertyName = "Logger";

        /// <summary>
        /// Enrich the log event.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var loggerName = "Default";

            if (logEvent.Properties.TryGetValue(Constants.SourceContextPropertyName, out var sourceContext))
            {
                if (sourceContext is ScalarValue sv && sv.Value is string)
                {
                    loggerName = (string)sv.Value;
                }
            }

            logEvent.AddPropertyIfAbsent(new LogEventProperty(LoggerNamePropertyName, new ScalarValue(loggerName)));
        }
    }
}