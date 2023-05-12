using APPLICATION.DOMAIN.CONTRACTS.CONFIGURATIONS;
using APPLICATION.DOMAIN.DTOS.RESPONSE.UTILS;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;

namespace APPLICATION.INFRAESTRUTURE.MIDDLEWARE
{
    public class GlobalExceptionHandlerMiddleware : IMiddleware
    {
        public readonly ILogWithMetric _logWithMetric;

        public GlobalExceptionHandlerMiddleware(ILogWithMetric logWithMetric)
        {
            _logWithMetric = logWithMetric;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logWithMetric.Error(ex);

                await HandleExceptionAsync(context);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var json = new ApiResponse<object>
            {
                Notificacoes = new List<DadosNotificacao>()
                {
                   new DadosNotificacao("Ocorreu um erro inesperado no sistema. Por favor tente novamente e caso o problema permaneça, entre em contato com o suporte.")
                }

            };

            return context.Response.WriteAsync(JsonConvert.SerializeObject(json));
        }
    }
}
