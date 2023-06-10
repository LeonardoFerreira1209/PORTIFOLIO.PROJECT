using APPLICATION.DOMAIN.DTOS.RESPONSE.UTILS;
using System.Net;

namespace APPLICATION.DOMAIN.EXCEPTIONS.USER;

/// <summary>
/// Exception customizado.
/// </summary>
public class CustomException : BaseException
{
    public CustomException(HttpStatusCode statusCode, object dados, List<DadosNotificacao> notificacoes)
    {
        Response = new ErrorResponse
            (statusCode, dados, notificacoes);
    }
}
