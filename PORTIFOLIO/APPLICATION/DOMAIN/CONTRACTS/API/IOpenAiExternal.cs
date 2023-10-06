using APPLICATION.DOMAIN.DTOS.REQUEST;
using APPLICATION.DOMAIN.DTOS.RESPONSE;
using Refit;

namespace APPLICATION.DOMAIN.CONTRACTS.API;

/// <summary>
/// Interface de acesso a API da OpenAI.
/// </summary>
public interface IOpenAiExternal
{
    /// <summary>
    /// Método de envio de para API de Completions da OPENAI.
    /// </summary>
    /// <param name="openAiCompletionsRequest"></param>
    /// <returns></returns>
    [Post("/completions")]
    Task<OpenAiCompletionsResponse> Completions([Body] OpenAiCompletionsRequest openAiCompletionsRequest);
}
