using System.Net.Mime;
using System.Text.Json;
using Clinic.Application.Common;

namespace Clinic_Project_Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BusinessRuleException ex)
        {
            _logger.LogWarning(ex, "Business rule violation: {Message}", ex.Message);

            await WriteProblemAsync(
                context,
                ex.StatusCode,
                ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");

            var detail = _env.IsDevelopment()
                ? ex.ToString()   // 👈 يظهر الخطأ الحقيقي أثناء التطوير
                : "An unexpected error occurred."; // 👈 آمن للإنتاج

            await WriteProblemAsync(
                context,
                StatusCodes.Status500InternalServerError,
                detail);
        }
    }

    private static async Task WriteProblemAsync(
        HttpContext context,
        int statusCode,
        string detail)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = MediaTypeNames.Application.Json;

        var response = new
        {
            title = statusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                _ => "Error"
            },
            status = statusCode,
            detail
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}