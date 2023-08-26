using APPLICATION.DOMAIN.ENTITY.ENTITY;

namespace APPLICATION.DOMAIN.ENTITY.CHAT;

/// <summary>
/// Entidade de Chat
/// </summary>
public class ChatEntity : Entity
{
    /// <summary>
    /// Id do promeiro usuário.
    /// </summary>
    public Guid FirstUserId { get; set; }

    /// <summary>
    /// Id do segundo usuário.
    /// </summary>
    public Guid SecondUserId { get; set; }

    /// <summary>
    /// Mensagens do chat.
    /// </summary>
    public virtual ICollection<ChatMessageEntity> Messages { get; set; }
}
