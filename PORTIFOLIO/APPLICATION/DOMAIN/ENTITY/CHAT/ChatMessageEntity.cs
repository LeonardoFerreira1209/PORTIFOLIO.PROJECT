using APPLICATION.DOMAIN.ENTITY.ENTITY;
using APPLICATION.DOMAIN.ENTITY.USER;

namespace APPLICATION.DOMAIN.ENTITY.CHAT;

/// <summary>
/// Entidade de mensagens do chat.
/// </summary>
public class ChatMessageEntity : Entity
{
    /// <summary>
    /// Id do chat.
    /// </summary>
    public Guid ChatId { get; set; }

    /// <summary>
    /// Id do usuário que enviou a mensagem.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Dados do usuário.
    /// </summary>
    public virtual UserEntity User { get; set; }

    /// <summary>
    /// Mensagem
    /// </summary>
    public string Message { get; set; }
}
