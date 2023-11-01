using Newtonsoft.Json;

namespace APPLICATION.DOMAIN.DTOS.RESPONSE;

/// <summary>
/// Classe de transporte de response do OpenAI Completions.
/// </summary>
public class OpenAiCompletionsResponse
{
    /// <summary>
    /// Id do request.
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// objeto.
    /// </summary>
    [JsonProperty("object")]
    public string Object {  get; set; }

    /// <summary>
    /// Data criação em TimeSpan.
    /// </summary>
    [JsonProperty("created")]
    public long Created { get; set; }

    /// <summary>
    /// Respostas da IA.
    /// </summary>
    [JsonProperty("choices")]
    public ICollection<OpenAICompletionsChoiceResponse> Choices { get; set; }

    /// <summary>
    /// Dados de uso da requisição.
    /// </summary>
    [JsonProperty("usage")]
    public OpenAiCompletionsUsageResponse Usage { get; set; }
}

/// <summary>
/// Dados da respostas de escolha.
/// </summary>
public class OpenAICompletionsChoiceResponse
{
    /// <summary>
    /// Index do array.
    /// </summary>
    [JsonProperty("index")]
    public int Index { get; set; }

    /// <summary>
    /// Dados da mensagem.
    /// </summary>
    [JsonProperty("message")]
    public OpenAiCompletionsMessageResponse openAiCompletionsMessageResponse { get; set; }

    /// <summary>
    /// Motivo de parada.
    /// </summary>
    [JsonProperty("finish_reason")]
    public string FinishReason { get; set; }
}

/// <summary>
/// Classe de dados da mensagem de retorno.
/// </summary>
public class OpenAiCompletionsMessageResponse
{
    /// <summary>
    /// responsável pela mensagem (system, user, assistant, function).
    /// </summary>
    [JsonProperty("role")]
    public string Role { get; set; }

    /// <summary>
    /// Conteudo da mensagem.
    /// </summary>
    [JsonProperty("content")]
    public string Content { get; set; }
}

/// <summary>
/// 
/// </summary>
public class OpenAiCompletionsUsageResponse
{
    /// <summary>
    /// Quantidades de tokens de envio.
    /// </summary>
    [JsonProperty("prompt_tokens")]
    public int PromptTokens { get; set; }

    /// <summary>
    /// Tokens para completar a mensagem de resposta.
    /// </summary>
    [JsonProperty("completion_tokens")]
    public int CompletionTokens { get; set; }

    /// <summary>
    /// Quantidade total de tokens usados.
    /// </summary>
    [JsonProperty("total_tokens")]
    public int TotalTokens { get; set; }
}