namespace APPLICATION.DOMAIN.DTOS.REQUEST.CHAT;

public class ChatHubMessageRequest
{
    /// <summary>
    /// Mensagem.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// File.
    /// </summary>
    public File File { get; set; }
}

public record File(string Name, string Type, string Content);
