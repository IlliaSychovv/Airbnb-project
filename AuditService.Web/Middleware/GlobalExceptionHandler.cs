using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace AuditService.Web.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }
    
    private static readonly Dictionary<Type, (int StatusCode, string Title)> ExceptionMap = new()
    {
        { typeof(ValidationException), ((int)HttpStatusCode.BadRequest, "Validation error") }
    };

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = ExceptionMap.TryGetValue(exception.GetType(), out var mapped)
            ? mapped
            : ((int)HttpStatusCode.InternalServerError, "Internal Server Error");
         
        if (statusCode == (int)HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Unhandled exception");
        else
            _logger.LogWarning(exception, "Handled Exception: {Title}", title);
        
        var problemDetails = new
        {
            status = statusCode,
            title,
            detail = exception.Message
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;  
    }
}