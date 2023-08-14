namespace logging.Middlewares;

using Serilog.Core;
using Serilog.Events;

public class RemovePropertiesEnricher: ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.RemovePropertyIfPresent("SourceContext");
        logEvent.RemovePropertyIfPresent("MessageTemplate");
        logEvent.RemovePropertyIfPresent("Properties");
        logEvent.RemovePropertyIfPresent("EventId");
    }
}