using Newtonsoft.Json;

namespace APPLICATION.DOMAIN.DTOS.REQUEST;

/// <summary>
/// Classe com dados de request para API do OPENAI imagens generation.
/// </summary>
public class OpenAiImagesGenerationRequest
{
    /// <summary>
    /// Modelo a ser usado. (dall-e-2 ou dall-e-3)
    /// </summary>
    [JsonProperty("model")]
    public string Model { get; set; }

    /// <summary>
    /// Prompt da imagem.
    /// </summary>
    [JsonProperty("prompt")]
    public string Prompt { get; set; }

    /// <summary>
    /// Quantidade de imagens.
    /// </summary>
    [JsonProperty("n")]
    public int N {  get; set; }

    /// <summary>
    /// Dimensão da imagem. Ex: 1024x1024.
    /// </summary>
    [JsonProperty("size")]
    public string Size { get; set; }
}