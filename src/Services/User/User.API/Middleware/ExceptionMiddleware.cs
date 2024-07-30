using Reservation.API.Models;
using System;
using System.Globalization;
using System.Net;
using User.Application.Exceptions;
using System.Text.Json;

namespace User.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }            
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
    {
        var statusCode = StatusCodes.Status500InternalServerError;
        var problem = new CustomProblemDetails();

        switch (ex)
        {
            case BadRequestException badRequestException:
                statusCode = StatusCodes.Status400BadRequest;
                problem = new CustomProblemDetails
                {
                    Title = badRequestException.Message,
                    Status = (int)statusCode,
                    Detail = badRequestException.InnerException?.Message,
                    Type = nameof(BadRequestException),
                    Errors = badRequestException.ValidationErrors
                };
                break;
            case NotFoundException notFoundException:
                statusCode = StatusCodes.Status404NotFound;
                problem = new CustomProblemDetails
                {
                    Title = notFoundException.Message,
                    Status = (int)statusCode,
                    Detail = notFoundException.InnerException?.Message,
                    Type = nameof(NotFoundException),
                };
                break;
            case AuthenticationException authenticationException:
                statusCode = StatusCodes.Status401Unauthorized;
                problem = new CustomProblemDetails
                {
                    Title = authenticationException.Message,
                    Status = (int)statusCode,
                    Detail = authenticationException.InnerException?.Message,
                    Type = nameof(NotFoundException),
                };
                break;
            default:
                statusCode = StatusCodes.Status500InternalServerError;
                problem = new CustomProblemDetails
                {
                    Title = ex.Message,
                    Status = (int)statusCode,
                    Detail = ex.StackTrace,
                    Type = nameof(NotFoundException),
                };
                break;
        }

        _logger.LogError(ex, "An error occurred while processing the command.");

        var jsonResponse = JsonSerializer.Serialize(problem);
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsync(jsonResponse);
    }
}
