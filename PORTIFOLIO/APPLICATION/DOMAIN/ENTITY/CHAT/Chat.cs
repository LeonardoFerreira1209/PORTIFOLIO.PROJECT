using APPLICATION.DOMAIN.ENTITY.BASE;
using APPLICATION.DOMAIN.ENTITY.USER;

namespace APPLICATION.DOMAIN.ENTITY.CHAT;

/// <summary>
/// Entidade de Chat
/// </summary>
public class Chat : Entity
{
    /// <summary>
    /// Id do promeiro usuário.
    /// </summary>
    public Guid FirstUserId { get; set; }

    /// <summary>
    /// Dados do primeiro usuário.
    /// </summary>
    public virtual User FirstUser { get; set; }

    /// <summary>
    /// Id do segundo usuário.
    /// </summary>
    public Guid SecondUserId { get; set; }

    /// <summary>
    /// Dados do segundo usuário.
    /// </summary>
    public virtual User SecondUser { get; set; }

    /// <summary>
    /// Mensagens do chat.
    /// </summary>
    public virtual ICollection<ChatMessage> Messages { get; set; }
}
