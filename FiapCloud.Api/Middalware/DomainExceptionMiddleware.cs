using System.Text.Json;
using FiapCLoud.Domain.Exceptions;

namespace FiapCloud.Api.Middalware;

public class DomainExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public DomainExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            context.Response.StatusCode = (int)ex.StatusCode;
            context.Response.ContentType = "application/json";

            var errorResponse = new
            {
                message = ex.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}