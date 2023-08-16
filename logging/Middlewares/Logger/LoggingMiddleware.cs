namespace logging.Middlewares.Logger;

using System.Text;
using System.Text.Json;
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
        SetCorrelationId(context);
        PushLoggingContextProperties(context);

        await LogRequestAsync(context.Request).ConfigureAwait(false);
        await InvokeNextLogAndHandleResponseAsync(context).ConfigureAwait(false);
    }

    private void SetCorrelationId(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString();
        context.Request.Headers["X-Correlation-ID"] = correlationId;
    }

    private static void PushLoggingContextProperties(HttpContext context)
    {
        LogContext.PushProperty("CorrelationId", context.Request.Headers["X-Correlation-ID"].FirstOrDefault());
        LogContext.PushProperty("ClientIp", context.Connection.RemoteIpAddress?.ToString() ?? "unknown");
        LogContext.PushProperty("Path", $"{context.Request.Path}{context.Request.QueryString}");
    }

    private async Task LogRequestAsync(HttpRequest request)
    {
        if (!_logger.IsEnabled(LogLevel.Information)) return;

        var requestBody = await BodyReader.ReadRequestBodyAsync(request).ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(requestBody))
        {
            _logger.LogInformation("{RequestBody}", requestBody);
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("{RequestHeader}", request.Headers);
        }
    }

    private async Task InvokeNextLogAndHandleResponseAsync(HttpContext context)
    {
        var originalResponseBody = context.Response.Body;

        using var memoryBodyStream = new MemoryStream();
        context.Response.Body = memoryBodyStream;

        await _next(context).ConfigureAwait(false);

        await LogResponseAsync(context.Response, memoryBodyStream).ConfigureAwait(false);
        
        await HandleServerErrorAsync(context.Response, memoryBodyStream).ConfigureAwait(false);
        
        memoryBodyStream.Position = 0;
        await memoryBodyStream.CopyToAsync(originalResponseBody).ConfigureAwait(false);
    }

    private async Task LogResponseAsync(HttpResponse response, MemoryStream responseBodyStream)
    {
        _logger.LogInformation("{ResponseStatusCode}", response.StatusCode);

        var responseBody = await BodyReader.ReadResponseBodyAsync(responseBodyStream).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(responseBody)) return;

        LogResponseBasedOnStatusCode(response.StatusCode, responseBody);
    }

    private void LogResponseBasedOnStatusCode(int statusCode, string responseBody)
    {
        switch (statusCode)
        {
            case >= 500: _logger.LogError("{ResponseBody}", responseBody);
                break;
            case >= 400: _logger.LogWarning("{ResponseBody}", responseBody);
                break;
            default: _logger.LogDebug("{ResponseBody}", responseBody);
                break;
        }
    }

    private static async Task HandleServerErrorAsync(HttpResponse response, Stream originBody)
    {
        if (response.StatusCode < 500) return;

        var responseBody = JsonSerializer.Serialize(new { code = 500, message = "Oops something went wrong :( try again later" });
        originBody.SetLength(0);
        using var memoryStreamModified = new MemoryStream(Encoding.UTF8.GetBytes(responseBody));
        await memoryStreamModified.CopyToAsync(originBody).ConfigureAwait(false);
    }
}