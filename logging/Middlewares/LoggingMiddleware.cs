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
        
        var requestBody = await ReadBody.GetRequestBody(context.Request);
        _logger.LogInformation("{Body}", requestBody);

        await _next.Invoke(context);

        var statusCode = context.Response.StatusCode;
        var responseBody = await ReadBody.GetResponseBody(memoryBodyStream);
        context.Response.Body = originalBody;

        if (statusCode >= 500)
        {
            _logger.LogCritical("{Body}", responseBody);
            var dict = await JsonSerializer.DeserializeAsync<IDictionary<string, object>>(memoryBodyStream);

            var code = "500";
            if (dict is not null && 
                dict.TryGetValue("code", out var _code))
            {
                code = _code.ToString();
            }
            var body = new {Code = code, Message = "Internal server error!"};
            context.Response.BodyWriter.Write(JsonSerializer.SerializeToUtf8Bytes(body));
        }
        else
        {
            await memoryBodyStream.CopyToAsync(originalBody);
            _logger.LogInformation("{Body}", responseBody);
        }
    }
}
