using System.Diagnostics;
using System.Net;

namespace Goblin.WebApp;

public class RequestLoggingMiddleware
{
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        var statusCode = context.Response.StatusCode;
        var statusName = ((HttpStatusCode)statusCode).ToString();
        using (_logger.BeginScope(new Dictionary<string, object?>
               {
                   ["request_headers"] = context.Request.Headers,
                   ["response_headers"] = context.Response.Headers
               }))
        {
            _logger.LogInformation(
                "Ответ {StatusCode} ({StatusName}) на запрос {Url} за {ElapsedMs} мс",
                statusCode,
                statusName,
                $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}",
                stopwatch.ElapsedMilliseconds);
        }
    }
}