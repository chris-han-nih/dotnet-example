using Serilog.Core;
using Serilog.Events;

namespace logging.Middlewares;

public class RemovePropertiesEnricher: ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.RemovePropertyIfPresent("SourceContext");
        logEvent.RemovePropertyIfPresent("MessageTemplate");
        logEvent.RemovePropertyIfPresent("Properties");
        logEvent.RemovePropertyIfPresent("EventId");
        logEvent.RemovePropertyIfPresent("RequestId");
        logEvent.RemovePropertyIfPresent("ConnectionId");
        logEvent.RemovePropertyIfPresent("RequestPath");
    }
}