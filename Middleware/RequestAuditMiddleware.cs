using System.Diagnostics;
using System.Security.Claims;

namespace InsuranceClaimsApi.Middleware;

public class RequestAuditMiddleware
{
    private const string CorrelationIdHeaderName = "X-Correlation-Id";

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestAuditMiddleware> _logger;

    public RequestAuditMiddleware(RequestDelegate next, ILogger<RequestAuditMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);
        var stopwatch = Stopwatch.StartNew();

        context.Response.Headers[CorrelationIdHeaderName] = correlationId;

        try
        {
            await _next(context);

            stopwatch.Stop();

            _logger.LogInformation(
                "Request completed. CorrelationId={CorrelationId}, Method={Method}, Path={Path}, StatusCode={StatusCode}, DurationMs={DurationMs}, Authenticated={Authenticated}, UserId={UserId}, Email={Email}",
                correlationId,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                context.User.Identity?.IsAuthenticated ?? false,
                GetUserId(context),
                GetEmail(context));
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "Request failed. CorrelationId={CorrelationId}, Method={Method}, Path={Path}, DurationMs={DurationMs}, Authenticated={Authenticated}, UserId={UserId}, Email={Email}",
                correlationId,
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds,
                context.User.Identity?.IsAuthenticated ?? false,
                GetUserId(context),
                GetEmail(context));

            throw;
        }
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var headerValue) &&
            !string.IsNullOrWhiteSpace(headerValue))
        {
            return headerValue.ToString();
        }

        return Guid.NewGuid().ToString("N");
    }

    private static string? GetUserId(HttpContext context)
    {
        return context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? context.User.FindFirstValue("sub");
    }

    private static string? GetEmail(HttpContext context)
    {
        return context.User.FindFirstValue(ClaimTypes.Email)
            ?? context.User.FindFirstValue("email");
    }
}
