using Serilog;

namespace SerilLogDemo.Helpers;

public class LogHelper
{
    public static string RequestPayload = "";

    public static async void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        var request = httpContext.Request;

        if (request.QueryString.HasValue)
            diagnosticContext.Set("QueryString", request.QueryString.Value);

        diagnosticContext.Set("RequestBody", RequestPayload);

        string responseBodyPayload = await ReadResponseBody(httpContext.Response);

        diagnosticContext.Set("ResponseBody", responseBodyPayload);
    }

    private static async Task<string> ReadResponseBody(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        string responseBody = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        return $"{responseBody}";
    }
}
