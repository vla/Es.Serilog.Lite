using System.Collections.Generic;
using Serilog.Events;

namespace Es.Serilog.Lite
{
    /// <summary>
    /// SourceContextFilterOptions
    /// </summary>
    public class SourceContextFilterOptions
    {
        /// <summary>
        ///  Gets or sets the minimum level of log messages.
        /// </summary>
        public LogEventLevel MinLevel { get; set; }

        /// <summary>
        /// Gets or sets the rules.
        /// </summary>
        public IList<SourceContextFilterRule> Rules { get; set; } = new List<SourceContextFilterRule>();
    }
}