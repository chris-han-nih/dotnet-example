namespace logging.Middlewares;

using System.Buffers;

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
        
        var requestBody = await ReadBody.GetRequestBody(context.Request);
        _logger.LogInformation("{Body}", requestBody);

        await _next.Invoke(context);

        var statusCode = context.Response.StatusCode;
        var responseBody = await ReadBody.GetResponseBody(memoryBodyStream);
        context.Response.Body = originalBody;
        
        if (statusCode >= 500)
        {
            _logger.LogCritical("{Body}", responseBody);
            context.Response.BodyWriter.Write("Internal server error!"u8);
        }
        if (statusCode >= 400)
        {
            _logger.LogWarning("{Body}", responseBody);
            context.Response.BodyWriter.Write("Something went wrong!"u8);
        }
        else
        {
            await memoryBodyStream.CopyToAsync(originalBody);
            _logger.LogInformation("{Body}", responseBody);
        }
    }
}
