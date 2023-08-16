namespace logging.Middlewares.Response;

using System.Text.Json;

public class ResponseMiddleware
{
    private readonly RequestDelegate _next;

    public ResponseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var originBody = context.Response.Body;
        var memStream = new MemoryStream();
        context.Response.Body = memStream;

        await _next(context).ConfigureAwait(false);

        var statusCode = context.Response.StatusCode;

        memStream.Position = 0;
        if (statusCode >= 500)
        {
            var responseBody = JsonSerializer.Serialize(new { code = 500, message = "Oops something went wrong :( try again later" });

            var memoryStreamModified = new MemoryStream();
            var sw = new StreamWriter(memoryStreamModified);
            await sw.WriteAsync(responseBody);
            await sw.FlushAsync();
            memoryStreamModified.Position = 0;

            await memoryStreamModified.CopyToAsync(originBody)
                                      .ConfigureAwait(false);
            context.Response.Body = originBody;
        }
        else
        {
            await memStream.CopyToAsync(originBody);
        }
    }
}