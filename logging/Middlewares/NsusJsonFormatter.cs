using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;

namespace logging.Middlewares;

public sealed class NsusJsonFormatter : ITextFormatter
{
    private readonly JsonValueFormatter _valueFormatter;

    /// <summary>
    /// Constructs a NsusJsonFormatter, optionally supplying a formatter for <see cref="LogEventPropertyValue"/>s on the event.
    /// </summary>
    /// <param name="valueFormatter">A value formatter, or null.</param>
    public NsusJsonFormatter(JsonValueFormatter? valueFormatter = null)
        => _valueFormatter = valueFormatter ?? new JsonValueFormatter(typeTagName: "$type");

    /// <summary>
    /// Formats the log event into the output. Subsequent events will be newline-delimited.
    /// </summary>
    /// <param name="logEvent">The event to format.</param>
    /// <param name="output">The output.</param>
    public void Format(LogEvent logEvent, TextWriter output)
    {
        if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
        if (output == null) throw new ArgumentNullException(nameof(output));

        FormatEvent(logEvent, output, _valueFormatter);
        output.WriteLine();
    }

    private static void FormatEvent(LogEvent logEvent, TextWriter output, JsonValueFormatter valueFormatter)
    {
        output.Write("{\"Timestamp\":\"");
        output.Write(logEvent.Timestamp.UtcDateTime.ToString("O"));
        
        AppendLevel(logEvent, output);
        AppendException(logEvent, output);
        AppendProperties(logEvent, output, valueFormatter);
        
        output.Write('}');
    }

    private static void AppendLevel(LogEvent logEvent, TextWriter output)
    {
        output.Write(",\"Level\":\"");
        output.Write(logEvent.Level);
        output.Write('\"');
    }

    private static void AppendException(LogEvent logEvent, TextWriter output)
    {
        if (logEvent.Exception == null) return;

        output.Write(",\"Exception\":");
        JsonValueFormatter.WriteQuotedJsonString(logEvent.Exception.ToString(), output);
    }

    private static void AppendProperties(LogEvent logEvent, TextWriter output, JsonValueFormatter valueFormatter)
    {
        foreach (var property in logEvent.Properties)
        {
            var name = property.Key;
            if (name.StartsWith('@'))
            {
                // Escape first '@' by doubling
                name = '@' + name;
            }

            output.Write(',');
            JsonValueFormatter.WriteQuotedJsonString(name, output);
            output.Write(':');
            valueFormatter.Format(property.Value, output);
        }
    }
}
