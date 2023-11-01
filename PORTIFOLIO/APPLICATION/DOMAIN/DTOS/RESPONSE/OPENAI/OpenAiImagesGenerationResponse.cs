using Newtonsoft.Json;

namespace APPLICATION.DOMAIN.DTOS.RESPONSE;

/// <summary>
/// Classe de transporte de response do OpenAI image generation.
/// </summary>
public class OpenAiImagesGenerationResponse
{
    /// <summary>
    /// Created.
    /// </summary>
    [JsonProperty("created")]
    public string Created { get; set; }

    /// <summary>
    /// Data
    /// </summary>
    [JsonProperty("data")]
    public List<DataResponse> Data { get; set; }
}

/// <summary>
/// data
/// </summary>
/// <param name="url"></param>
public record DataResponse(string url);