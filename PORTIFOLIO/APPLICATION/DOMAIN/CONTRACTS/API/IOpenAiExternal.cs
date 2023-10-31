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
    [Post("chat/completions")]
    Task<OpenAiCompletionsResponse> Completions([Body] OpenAiCompletionsRequest openAiCompletionsRequest);

    /// <summary>
    /// Método de envio de para API de imagens da OPENAI.
    /// </summary>
    /// <param name="openAiImagesGenerationRequest"></param>
    /// <returns></returns>
    [Post("images/generations")]
    Task<OpenAiImagesGenerationResponse> ImageGeneration([Body] OpenAiImagesGenerationRequest openAiImagesGenerationRequest);
}
