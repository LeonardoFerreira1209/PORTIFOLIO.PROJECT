using APPLICATION.DOMAIN.DTOS.RESPONSE.USER;
using APPLICATION.DOMAIN.ENUMS;

namespace APPLICATION.DOMAIN.DTOS.RESPONSE.CHAT;

/// <summary>
/// Classe de response de chat messages.
/// </summary>
public class ChatMessageResponse
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Data de atualização
    /// </summary>
    public DateTime? Updated { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    public virtual Status Status { get; set; }

    /// <summary>
    /// Id do usuário que enviou a mensagem.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Dados do usuário.
    /// </summary>
    public UserResponse UserToSendMessage { get; set; }

    /// <summary>
    /// Id do chat em que a mensagem está vinculada.
    /// </summary>
    public Guid ChatId { get; set; }

    /// <summary>
    /// Dados do chat em que a mensagem está vinculada.
    /// </summary>
    public ChatResponse Chat { get; set; }

    /// <summary>
    /// Mensagem
    /// </summary>
    public string Message { get; set; }
}
