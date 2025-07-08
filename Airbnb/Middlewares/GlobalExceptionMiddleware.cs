using System.Net;
using System.Text.Json;

namespace Airbnb.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occured");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode status;
        string message;

        if (exception is InvalidOperationException)
        {
            status = HttpStatusCode.BadRequest;
            message = exception.Message;
        }
        else
        {
            status = HttpStatusCode.InternalServerError;
            message = exception.Message;
        }

        var response = new { status = (int)status, message };
        
        var load = JsonSerializer.Serialize(response);
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;
        
        await context.Response.WriteAsync(load);
    }
} 