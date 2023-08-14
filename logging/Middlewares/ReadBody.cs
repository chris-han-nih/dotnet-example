namespace logging.Middlewares;

using System.Text;

internal static class ReadBody
{
    internal static async Task<string> GetRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();

        var body = request.Body;
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        _ = await request.Body.ReadAsync(buffer);
        var requestBody = Encoding.UTF8.GetString(buffer);
        body.Seek(0, SeekOrigin.Begin);
        request.Body = body;

        return requestBody;
    }

    internal static async Task<string> GetResponseBodyAsync(MemoryStream memoryBodyStream)
    {
        memoryBodyStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(memoryBodyStream).ReadToEndAsync();
        memoryBodyStream.Seek(0, SeekOrigin.Begin);
        
        return responseBody;
    }
}