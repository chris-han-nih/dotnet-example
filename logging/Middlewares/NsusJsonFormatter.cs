namespace logging.Middlewares;

using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;

public sealed class NsusJsonFormatter : ITextFormatter
{
    private readonly JsonValueFormatter _valueFormatter;

    /// <summary>
    /// Construct a <see cref="CompactJsonFormatter"/>, optionally supplying a formatter for
    /// <see cref="LogEventPropertyValue"/>s on the event.
    /// </summary>
    /// <param name="valueFormatter">A value formatter, or null.</param>
    public NsusJsonFormatter(JsonValueFormatter valueFormatter = null)
        => _valueFormatter = valueFormatter ?? new JsonValueFormatter(typeTagName: "$type");

    /// <summary>
    /// Format the log event into the output. Subsequent events will be newline-delimited.
    /// </summary>
    /// <param name="logEvent">The event to format.</param>
    /// <param name="output">The output.</param>
    public void Format(LogEvent logEvent, TextWriter output)
    {
        NsusJsonFormatter.FormatEvent(logEvent, output, _valueFormatter);
        output.WriteLine();
    }

    /// <summary>
    /// Format the log event into the output.
    /// </summary>
    /// <param name="logEvent">The event to format.</param>
    /// <param name="output">The output.</param>
    /// <param name="valueFormatter">A value formatter for <see cref="LogEventPropertyValue"/>s on the event.</param>
    private static void FormatEvent(LogEvent logEvent, TextWriter output, JsonValueFormatter valueFormatter)
    {
        if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
        if (output == null) throw new ArgumentNullException(nameof(output));
        if (valueFormatter == null) throw new ArgumentNullException(nameof(valueFormatter));

        output.Write("{\"Timestamp\":\"");
        output.Write(logEvent.Timestamp.UtcDateTime.ToString("O"));

        output.Write(",\"Level\":\"");
        output.Write(logEvent.Level);
        output.Write('\"');

        if (logEvent.Exception != null)
        {
            output.Write(",\"Exception\":");
            JsonValueFormatter.WriteQuotedJsonString(logEvent.Exception.ToString(), output);
        }

        foreach (var property in logEvent.Properties)
        {
            var name = property.Key;
            if (name.Length > 0
             && name[0] == '@')
            {
                // Escape first '@' by doubling
                name = '@' + name;
            }
        
            output.Write(',');
            JsonValueFormatter.WriteQuotedJsonString(name, output);
            output.Write(':');
            valueFormatter.Format(property.Value, output);
        }

        output.Write('}');
    }
}