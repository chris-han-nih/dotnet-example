namespace logging.Middlewares;

using Serilog.Context;

public class LoggingMiddleware
{
    private readonly ILogger _logger;
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        LoggingMiddleware.PushLoggingContextProperties(context);

        await LogRequestAsync(context.Request);
        
        await InvokeNextAndLogResponseAsync(context);
    }

    private static void PushLoggingContextProperties(HttpContext context)
    {
        LogContext.PushProperty("CorrelationId", context.Request.Headers["X-Correlation-ID"].FirstOrDefault());
        LogContext.PushProperty("ClientIp", context.Connection.RemoteIpAddress?.ToString() ?? "unknown");
        LogContext.PushProperty("RequestPath", context.Request.Path + context.Request.QueryString);
    }

    private async Task LogRequestAsync(HttpRequest request)
    {
        if (!_logger.IsEnabled(LogLevel.Information)) return;
        
        var requestBody = await BodyReader.ReadRequestBodyAsync(request);
        if (!string.IsNullOrEmpty(requestBody))
        {
            _logger.LogInformation("RequestBody: {Body}", requestBody);
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("RequestHeaders: {Headers}", request.Headers);
        }
    }

    private async Task InvokeNextAndLogResponseAsync(HttpContext context)
    {
        var originalResponseBody = context.Response.Body;

        using var memoryBodyStream = new MemoryStream();
        context.Response.Body = memoryBodyStream;

        await _next.Invoke(context);

        await LogResponse(context.Response, memoryBodyStream);

        memoryBodyStream.Position = 0;
        await memoryBodyStream.CopyToAsync(originalResponseBody);
    }

    private async Task LogResponse(HttpResponse response, MemoryStream responseBodyStream)
    {
        _logger.LogInformation("ResponseStatusCode: {StatusCode}", response.StatusCode);

        var responseBody = await BodyReader.ReadResponseBodyAsync(responseBodyStream);
        _logger.LogInformation("ResponseBody: {Body}", responseBody);
    }
}
