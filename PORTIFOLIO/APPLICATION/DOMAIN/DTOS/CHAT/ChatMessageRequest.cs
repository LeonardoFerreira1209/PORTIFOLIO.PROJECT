namespace APPLICATION.DOMAIN.DTOS.CHAT;

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
    /// Id do chat em que a mensagem está vinculada.
    /// </summary>
    public Guid ChatId { get; set; }
}
