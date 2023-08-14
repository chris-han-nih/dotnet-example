namespace logging.Middlewares;

using System.Buffers;
using System.Text.Json;

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
        var originalBody = context.Response.Body;
        var memoryBodyStream = new MemoryStream();
        context.Response.Body = memoryBodyStream;

        await LoggingRequestAsync(context.Request);
        
        await _next.Invoke(context);
        
        await LoggingResponseAsync(context.Response, memoryBodyStream);
        
        await memoryBodyStream.CopyToAsync(originalBody);
    }

    private async Task LoggingRequestAsync(HttpRequest request)
    {
        if (!_logger.IsEnabled(LogLevel.Information)) return;

        var queryString = request.QueryString;
        _logger.LogInformation("{QueryString}", queryString);

        var body = await ReadBody.GetRequestBodyAsync(request);
        if (string.IsNullOrEmpty(body)) return;

        _logger.LogInformation("{Body}", body);
        
        if (!_logger.IsEnabled(LogLevel.Debug)) return;
        _logger.LogDebug("{Headers}", request.Headers);
    }

    private async Task LoggingResponseAsync(HttpResponse response, MemoryStream responseBodyStream)
    {
        var statusCode = response.StatusCode;
        var body = await ReadBody.GetResponseBodyAsync(responseBodyStream);

        _logger.LogInformation("{StatusCode}", statusCode);
        _logger.LogInformation("{Body}", body);
    }
}
