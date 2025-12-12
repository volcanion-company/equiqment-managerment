using System.Diagnostics;
using System.Text;

namespace EquipmentManagement.WebAPI.Middleware;

/// <summary>
/// Middleware that logs HTTP request and response information, including method, path, status code, and body content,
/// for each processed request.
/// </summary>
/// <remarks>This middleware captures and logs the full request and response bodies, which may have performance
/// and security implications for large payloads or sensitive data. Use caution when enabling in production
/// environments. The middleware should be registered early in the pipeline to capture all relevant
/// information.</remarks>
/// <param name="next">The next middleware delegate in the HTTP request pipeline. This cannot be null.</param>
/// <param name="logger">The logger used to record request and response details. This cannot be null.</param>
public class RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
{
    /// <summary>
    /// Processes an HTTP request, logging request and response details asynchronously as part of the middleware
    /// pipeline.
    /// </summary>
    /// <remarks>This method should be called within the ASP.NET Core middleware pipeline. It logs both the
    /// incoming request and the outgoing response, including the time taken to process the request. The method
    /// temporarily replaces the response body stream to capture response data for logging purposes before restoring the
    /// original stream.</remarks>
    /// <param name="context">The HTTP context for the current request. Provides access to request and response information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // Start timing
        var stopwatch = Stopwatch.StartNew();
        // Log request
        await LogRequest(context);
        // Copy the original response body stream
        var originalBodyStream = context.Response.Body;
        // Create a new memory stream to capture the response
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        // Call the next middleware in the pipeline
        await next(context);
        // Stop timing
        stopwatch.Stop();
        // Log response
        await LogResponse(context, stopwatch.ElapsedMilliseconds);
        // Copy the contents of the new memory stream (which contains the response) to the original stream
        await responseBody.CopyToAsync(originalBodyStream);
    }

    /// <summary>
    /// Logs the HTTP request method, path, and body content for the specified HTTP context.
    /// </summary>
    /// <remarks>This method enables request body buffering to allow reading the request body without
    /// interfering with subsequent middleware. The request body is read as a UTF-8 encoded string. If the request has
    /// no body, an empty string is logged for the body content.</remarks>
    /// <param name="context">The HTTP context containing the request to be logged. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous logging operation.</returns>
    private async Task LogRequest(HttpContext context)
    {
        // Enable buffering to allow reading the request body multiple times
        context.Request.EnableBuffering();
        // Read the request body
        var requestBody = string.Empty;
        if (context.Request.ContentLength > 0)
        {
            // Reset the request body stream position to the beginning
            using var reader = new StreamReader(context.Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            // Read the body asynchronously
            requestBody = await reader.ReadToEndAsync();
            // Reset the stream position again for downstream middleware
            context.Request.Body.Position = 0;
        }
        // Log the request details
        logger.LogInformation("HTTP {Method} {Path} Request: {Body}", context.Request.Method, context.Request.Path, requestBody);
    }

    /// <summary>
    /// Asynchronously logs the HTTP response details, including status code, elapsed time, and response body, for the
    /// specified HTTP context.
    /// </summary>
    /// <remarks>The response body stream position is reset before and after reading to ensure subsequent
    /// middleware or components can access the response body as expected.</remarks>
    /// <param name="context">The HTTP context containing the response to be logged. Must not be null.</param>
    /// <param name="elapsedMilliseconds">The total time, in milliseconds, that elapsed during the processing of the HTTP request.</param>
    /// <returns>A task that represents the asynchronous logging operation.</returns>
    private async Task LogResponse(HttpContext context, long elapsedMilliseconds)
    {
        // Reset the response body stream position to the beginning
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        // Read the response body
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        // Reset the stream position again for downstream middleware
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        // Log the response details
        logger.LogInformation("HTTP {Method} {Path} Response: {StatusCode} in {ElapsedMilliseconds}ms - {Body}", context.Request.Method, context.Request.Path, context.Response.StatusCode, elapsedMilliseconds, responseBody);
    }
}
