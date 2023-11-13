namespace APPLICATION.DOMAIN.DTOS.REQUEST.CHAT;

/// <summary>
/// Classe de criação de mensagem do chat.
/// </summary>
public class ChatMessageRequest
{
    /// <summary>
    /// Id do usuário que enviou a mensagem.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Mensagem do usuário.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Imagens.
    /// </summary>
    public List<File> Images { get; set; }

    /// <summary>
    /// Id do chat em que a mensagem está vinculada.
    /// </summary>
    public Guid ChatId { get; set; }

    /// <summary>
    /// Se a mensagem tem um commando.
    /// </summary>
    public bool HasCommand { get; set; }

    /// <summary>
    /// Comandos de interação.
    /// </summary>
    public string Command { get; set; }

    /// <summary>
    /// A mensagem foi gerada por um chatBot. 
    /// </summary>
    public bool IsChatBot { get; set; }

    /// <summary>
    /// É uma mensagem. 
    /// </summary>
    public bool IsImage { get; set; }

    /// <summary>
    /// Url do arquivo.
    /// </summary>
    public string Url { get; set; }
}
