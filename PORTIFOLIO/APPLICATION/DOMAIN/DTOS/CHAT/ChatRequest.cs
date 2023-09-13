namespace APPLICATION.DOMAIN.DTOS.CHAT;

/// <summary>
/// Classe de criação de chat.
/// </summary>
public class ChatRequest
{
    /// <summary>
    /// Id do promeiro usuário.
    /// </summary>
    public Guid FirstUserId { get; set; }

    /// <summary>
    /// Id do segundo usuário.
    /// </summary>
    public Guid SecondUserId { get; set; }
}
