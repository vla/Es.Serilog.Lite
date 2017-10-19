using System;
using System.IO;
using System.Linq;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;
using Serilog.Parsing;

namespace Es.Serilog.Lite.Formatting
{
    /// <summary>
    /// JsonFormatter
    /// </summary>
    public class JsonFormatter : ITextFormatter
    {
        private readonly JsonValueFormatter _valueFormatter;

        /// <summary>
        /// Construct a <see cref="JsonFormatter"/>, optionally supplying a formatter for
        /// <see cref="LogEventPropertyValue"/>s on the event.
        /// </summary>
        /// <param name="valueFormatter">A value formatter, or null.</param>
        public JsonFormatter(JsonValueFormatter valueFormatter = null)
        {
            _valueFormatter = valueFormatter ?? new JsonValueFormatter(typeTagName: "$type");
        }

        /// <summary>
        /// Format the log event into the output. Subsequent events will be newline-delimited.
        /// </summary>
        /// <param name="logEvent">The event to format.</param>
        /// <param name="output">The output.</param>
        public void Format(LogEvent logEvent, TextWriter output)
        {
            FormatEvent(logEvent, output, _valueFormatter);
            output.WriteLine();
        }

        /// <summary>
        /// Format the log event into the output.
        /// </summary>
        /// <param name="logEvent">The event to format.</param>
        /// <param name="output">The output.</param>
        /// <param name="valueFormatter">A value formatter for <see cref="LogEventPropertyValue"/>s on the event.</param>
        public static void FormatEvent(LogEvent logEvent, TextWriter output, JsonValueFormatter valueFormatter)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            if (output == null) throw new ArgumentNullException(nameof(output));
            if (valueFormatter == null) throw new ArgumentNullException(nameof(valueFormatter));

            output.Write("{\"AppTime\":\"");
            output.Write(logEvent.Timestamp.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss,fff"));
            output.Write("\",\"Message\":");

            var message = logEvent.MessageTemplate.Render(logEvent.Properties);
            JsonValueFormatter.WriteQuotedJsonString(message, output);

            output.Write(",\"Level\":\"");
            output.Write(GetLevel(logEvent.Level));
            output.Write('\"');

            if (logEvent.Exception != null)
            {
                output.Write(",\"Exception\":");
                JsonValueFormatter.WriteQuotedJsonString(logEvent.Exception.ToString(), output);
            }

            //filter props
            var propTokens = logEvent.MessageTemplate.Tokens.OfType<PropertyToken>();
            var props = logEvent.Properties.Keys;
            if (propTokens.Any())
            {
                props = props.Except(propTokens.Select(s => s.PropertyName));
            }

            foreach (var key in props)
            {
                var name = key;

                var property = logEvent.Properties[name];

                if (name.Length > 0 && name[0] == '@')
                {
                    // Escape first '@' by doubling
                    name = '@' + name;
                }

                output.Write(',');
                JsonValueFormatter.WriteQuotedJsonString(name, output);
                output.Write(':');
                valueFormatter.Format(property, output);
            }

            output.Write('}');
        }

        private static string GetLevel(LogEventLevel level)
        {
            switch (level)
            {
                case LogEventLevel.Debug:
                    return "debug";

                case LogEventLevel.Information:
                    return "info";

                case LogEventLevel.Verbose:
                    return "verbose";

                case LogEventLevel.Warning:
                    return "warn";

                case LogEventLevel.Error:
                    return "error";

                case LogEventLevel.Fatal:
                    return "fatal";
            }
            return string.Empty;
        }
    }
}