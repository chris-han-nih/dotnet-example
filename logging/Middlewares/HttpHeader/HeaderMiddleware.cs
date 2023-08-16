namespace logging.Middlewares.HttpHeader;

public class HeaderMiddleware
{
    private readonly RequestDelegate _next;

    public HeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task Invoke(HttpContext context)
    {
        context.Request.Headers.Add("X-Correlation-ID", context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString());
        
        await _next(context).ConfigureAwait(false);
    }
}