using Microsoft.Extensions.Primitives;
using System.Diagnostics;

namespace FiapCloud.Api.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;
        private readonly string _headerName = "X-Correlation-ID";

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string correlationId = GetOrGenerateCorrelationId(context);

            context.Response.Headers[_headerName] = correlationId;

            if (Activity.Current != null)
            {
                Activity.Current.SetTag("correlation_id", correlationId);
            }

            using (_logger.BeginScope("CorrelationId:{CorrelationId}", correlationId))
            {
                _logger.LogDebug("Correlation ID definido para este request.");

                try
                {
                    await _next(context);
                }
                catch
                {
                    if (!context.Response.Headers.ContainsKey(_headerName))
                    {
                        context.Response.Headers[_headerName] = correlationId;
                    }
                    throw;
                }
            }
        }

        private string GetOrGenerateCorrelationId(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(_headerName, out StringValues values))
            {
                var headerCorrelationId = values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
                if (!string.IsNullOrEmpty(headerCorrelationId))
                {
                    context.TraceIdentifier = headerCorrelationId;
                    return headerCorrelationId;
                }
            }

            var newCorrelationId = Guid.NewGuid().ToString();
            context.TraceIdentifier = newCorrelationId;
            return newCorrelationId;
        }
    }
}
