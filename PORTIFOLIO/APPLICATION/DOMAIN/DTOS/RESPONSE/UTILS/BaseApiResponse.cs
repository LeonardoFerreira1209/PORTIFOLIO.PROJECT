using System.Net;

namespace APPLICATION.DOMAIN.DTOS.RESPONSE.UTILS;

/// <summary>
/// Dados a ser retornado em uma notificação do sistema.
/// </summary>
public class DadosNotificacao
{
    public DadosNotificacao(string mensagem) { Mensagem = mensagem; }

    /// <summary>
    /// Mensagem da notificação.
    /// </summary>
    public string Mensagem { get; }
}

/// <summary>
/// Classe 
/// </summary>
public abstract class BaseApiResponse
{
    public BaseApiResponse(HttpStatusCode statusCode) {
        StatusCode = statusCode;
    }

    public BaseApiResponse(HttpStatusCode statusCode, bool sucesso, List<DadosNotificacao> notificacoes)
    {
        StatusCode = statusCode;
        Sucesso = sucesso;
        Notificacoes = notificacoes;
    }

    public BaseApiResponse(HttpStatusCode statusCode, bool sucesso, object dados, List<DadosNotificacao> notificacoes) {
        StatusCode = statusCode;
        Sucesso = sucesso;
        Dados = dados;
        Notificacoes = notificacoes;
    }

    /// <summary>
    /// Status code.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Retorna true se a requisição para API foi bem sucedida.
    /// </summary>
    public bool Sucesso { get; }

    /// <summary>
    /// Dados a serem retornados na requisição.
    /// </summary>
    public object Dados { get; }

    /// <summary>
    /// Notificações que retornam da requisição, sejam elas Sucesso, Erro, Informação.
    /// </summary>
    public List<DadosNotificacao> Notificacoes { get; }
}
