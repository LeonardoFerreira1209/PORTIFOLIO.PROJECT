using APPLICATION.DOMAIN.DTOS.RESPONSE.BASE;
using System.Net;

namespace APPLICATION.DOMAIN.EXCEPTIONS;

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
