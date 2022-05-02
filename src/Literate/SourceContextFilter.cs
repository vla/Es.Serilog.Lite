using System;
using Serilog.Core;
using Serilog.Events;

namespace Es.Serilog.Lite
{
    /// <summary>
    /// SourceContext Filter
    /// </summary>
    /// <seealso cref="ILogEventFilter" />
    public class SourceContextFilter : ILogEventFilter
    {
        private readonly SourceContextFilterOptions _filterOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceContextFilter" /> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public SourceContextFilter(SourceContextFilterOptions options)
        {
            _filterOptions = options;
        }

        /// <summary>
        /// Returns true if the provided event is enabled. Otherwise, false.
        /// </summary>
        /// <param name="logEvent">The event to test.</param>
        /// <returns>
        /// True if the event is enabled by this filter. If false
        /// is returned, the event will not be emitted.
        /// </returns>
        public bool IsEnabled(LogEvent logEvent)
        {
            if (_filterOptions.Rules.Count == 0)
                return true;

            if (logEvent.Properties.TryGetValue(Constants.SourceContextPropertyName, out var sourceContext))
            {
                if (sourceContext is ScalarValue sv && sv.Value is string)
                {
                    var loggerName = (string)sv.Value;

                    foreach (var rule in _filterOptions.Rules)
                    {
                        if (loggerName.StartsWith(rule.SourceContextName))
                        {
                            if (logEvent.Level <= _filterOptions.MinLevel)
                            {
                                return false;
                            }

                            if (rule.LogLevel.HasValue && logEvent.Level <= rule.LogLevel.Value)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }
    }
}