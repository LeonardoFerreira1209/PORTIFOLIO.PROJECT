using System.Net;

namespace APPLICATION.DOMAIN.DTOS.RESPONSE.UTILS;

/// <summary>
/// Retorno das APIS com erro.
/// </summary>
public class ErrorResponse : BaseApiResponse
{
    public ErrorResponse(HttpStatusCode statusCode, List<DadosNotificacao> notificacoes = null) : base(statusCode, false, notificacoes)
    {

    }

    public ErrorResponse(HttpStatusCode statusCode, object dados = null, List<DadosNotificacao> notificacoes = null) : base(statusCode, false, dados, notificacoes)
    {

    }
}
