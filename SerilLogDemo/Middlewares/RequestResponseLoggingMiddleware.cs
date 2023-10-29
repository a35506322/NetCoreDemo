using Microsoft.AspNetCore.Razor.TagHelpers;
using SerilLogDemo.Helpers;
using System.Text;

namespace SerilLogDemo.Middlewares;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _config;

    public RequestResponseLoggingMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _config = config;
    }

    public async Task Invoke(HttpContext context)
    {
        // Read and log request body data
        string requestBodyPayload = await ReadRequestBody(context.Request);

        LogHelper.RequestPayload = requestBodyPayload;

        // Read and log response body data
        // Copy a pointer to the original response body stream
        var originalResponseBodyStream = context.Response.Body;

        // Create a new memory stream...
        using (var responseBody = new MemoryStream())
        {
            // ...and use that for the temporary response body
            context.Response.Body = responseBody;

            // Continue down the Middleware pipeline, eventually returning to this class
            await _next(context);

            // Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
            await responseBody.CopyToAsync(originalResponseBodyStream);
        }
    }

    private async Task<string> ReadRequestBody(HttpRequest request)
    {
        HttpRequestRewindExtensions.EnableBuffering(request);

        var body = request.Body;
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        await request.Body.ReadAsync(buffer, 0, buffer.Length);
        string requestBody = Encoding.UTF8.GetString(buffer);
        body.Seek(0, SeekOrigin.Begin);
        request.Body = body;

        return $"{requestBody}";
    }
}
