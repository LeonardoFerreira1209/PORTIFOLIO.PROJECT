using APPLICATION.DOMAIN.DTOS.RESPONSE.UTILS;
using APPLICATION.DOMAIN.EXCEPTIONS;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

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

        var (statusCode, json) = 
            GenerateResponse(exception);

        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(json);
    }

    private static (HttpStatusCode statusCode, string json) GenerateResponse(Exception exception)
        => exception switch
        {
            BaseException customEx => (customEx.Response.StatusCode,
                                                           JsonSerializer.Serialize(customEx.Response)),

            _ => (HttpStatusCode.InternalServerError, JsonSerializer.Serialize(new 
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Sucesso = false,
                Dados = new
                {
                    exception.StackTrace,
                    exception.Message,
                    InnerException = exception.InnerException?.ToString(),
                    exception.Data
                },
                Notificacoes = new DadosNotificacao(exception.Message),
            }))
        };
}
