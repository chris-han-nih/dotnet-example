namespace logging.Middlewares;

using System.Text;

internal static class BodyReader
{
    internal static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();

        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        _ = await request.Body.ReadAsync(buffer);
        request.Body.Seek(0, SeekOrigin.Begin);

        return Encoding.UTF8.GetString(buffer);
    }

    internal static async Task<string> ReadResponseBodyAsync(MemoryStream memoryBodyStream)
    {
        memoryBodyStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(memoryBodyStream).ReadToEndAsync();
        memoryBodyStream.Seek(0, SeekOrigin.Begin);
        
        return responseBody;
    }
}