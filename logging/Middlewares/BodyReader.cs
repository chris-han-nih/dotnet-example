using System.Text;

namespace logging.Middlewares;

internal static class BodyReader
{
    internal static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();

        if (request.ContentLength is null or <= 0) return string.Empty;

        try
        {
            var buffer = new byte[request.ContentLength.Value];
            var bytesRead = await request.Body.ReadAsync(buffer);

            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }
        catch (Exception)
        {
            return string.Empty;
        }
        finally
        {
            request.Body.Seek(0, SeekOrigin.Begin);
        }
    }

    internal static async Task<string> ReadResponseBodyAsync(MemoryStream memoryBodyStream)
    {
        if (memoryBodyStream.Length <= 0) return string.Empty;
        
        memoryBodyStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(memoryBodyStream).ReadToEndAsync();
        memoryBodyStream.Seek(0, SeekOrigin.Begin);
        
        return responseBody;
    }
}