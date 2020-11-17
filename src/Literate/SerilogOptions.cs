using Es.Serilog.Lite.Email;
using Serilog.Events;

namespace Es.Serilog.Lite
{
    /// <summary>
    /// Serilog Options
    /// </summary>
    public class SerilogOptions
    {
        /// <summary>
        /// Whether to enable standard output.
        /// </summary>
        public bool StdOut { get; set; }

        /// <summary>
        /// Whether to enable logging to serialize json.
        /// </summary>
        public bool FormatJson { get; set; }

        /// <summary>
        /// Gets or sets the source context filter options.
        /// </summary>
        public SourceContextFilterOptions SourceContextFilterOptions { get; set; }

        /// <summary>
        /// Whether to enable file records.
        /// </summary>
        public bool RollingFile { get; set; }

        /// <summary>
        /// the IO output is asynchronous.
        /// </summary>
        public bool Async { get; set; }

        /// <summary>
        /// File log format file path.
        /// </summary>
        public string PathFormat { get; set; }

        /// <summary>
        /// File log file path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Log output template
        /// </summary>
        public string OutputTemplate { get; set; }

        /// <summary>
        /// Minimum log level
        /// </summary>
        public LogEventLevel? LogMinLevel { get; set; }

        /// <summary>
        /// Gets or sets the email configuration.
        /// </summary>
        public EmailConfig Email { get; set; }
    }
}