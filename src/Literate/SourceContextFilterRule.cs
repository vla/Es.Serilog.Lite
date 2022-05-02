using Serilog.Events;

namespace Es.Serilog.Lite
{
    /// <summary>
    /// SourceContextFilterRule
    /// </summary>
    public class SourceContextFilterRule
    {
        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        public LogEventLevel? LogLevel { get; set; }

        /// <summary>
        /// Gets the name of the source context. Use StartsWith matching.
        /// </summary>
        public string SourceContextName { get; set; } = default!;
    }
}