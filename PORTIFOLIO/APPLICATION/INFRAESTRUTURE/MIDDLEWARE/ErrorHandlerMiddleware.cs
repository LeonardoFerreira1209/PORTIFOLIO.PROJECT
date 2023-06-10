using APPLICATION.DOMAIN.EXCEPTIONS;
using APPLICATION.DOMAIN.EXCEPTIONS.USER;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using static APPLICATION.DOMAIN.EXCEPTIONS.USER.CustomUserException;

namespace APPLICATION.INFRAESTRUTURE.MIDDLEWARE;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _requestDelegate;

    public ErrorHandlerMiddleware(
        RequestDelegate requestDelegate)
    {
        _requestDelegate = requestDelegate;
    }

    public async Task InvokeAsync(
        HttpContext context)
    {
        try
        {
            await _requestDelegate(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    protected static Task HandleExceptionAsync(
        HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var (statusCode, json) = GenerateResponse(exception);
        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(json);
    }

    private static (HttpStatusCode statusCode, string json) GenerateResponse(Exception exception)
        => exception switch
        {
            BaseException customEx => (customEx.Response.StatusCode,
                                                           JsonSerializer.Serialize(customEx.Response)),

            _ => (HttpStatusCode.InternalServerError, JsonSerializer.Serialize(new { error = exception.Message }))
        };
}
