using Newtonsoft.Json;

namespace APPLICATION.DOMAIN.DTOS.REQUEST;

/// <summary>
/// Classe com dados de request para API do OPENAI Completions.
/// </summary>
public class OpenAiCompletionsRequest
{
    /// <summary>
    /// modelo da IA.
    /// </summary>
    [JsonProperty("model")]
    public string Model { get; set; }

    /// <summary>
    /// Mensagens.
    /// </summary>
    [JsonProperty("messages")]
    public List<OpenAiCompletionsMessagesRequest> Messages { get; set; }
}

/// <summary>
/// Classe com dados da mensagem para request para API do OPENAI Completions. 
/// </summary>
public class OpenAiCompletionsMessagesRequest
{
    /// <summary>
    /// responsável pela mensagem (system, user, assistant, function);
    /// </summary>
    [JsonProperty("role")]
    public string Role { get; set; }

    /// <summary>
    /// Conteudo da mensagem.
    /// </summary>
    [JsonProperty("content")]
    public string Content { get; set; }
}
