using APPLICATION.DOMAIN.ENTITY.BASE;
using APPLICATION.DOMAIN.ENTITY.USER;

namespace APPLICATION.DOMAIN.ENTITY.CHAT;

/// <summary>
/// Entidade de mensagens do chat.
/// </summary>
public class ChatMessage : Entity
{
    /// <summary>
    /// Id do usuário que enviou a mensagem.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Dados do usuário.
    /// </summary>
    public virtual User UserToSendMessage { get; set; }

    /// <summary>
    /// Id do chat em que a mensagem está vinculada.
    /// </summary>
    public Guid ChatId { get; set; }

    /// <summary>
    /// Dados do chat em que a mensagem está vinculada.
    /// </summary>
    public virtual Chat Chat { get; set; }

    /// <summary>
    /// Mensagem
    /// </summary>
    public string Message { get; set; }
}
