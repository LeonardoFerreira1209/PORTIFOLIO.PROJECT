using APPLICATION.DOMAIN.ENUMS;

namespace APPLICATION.DOMAIN.DTOS.RESPONSE.CHAT;

/// <summary>
/// Classe de response de chat.
/// </summary>
public class ChatResponse
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
    /// Id do promeiro usuário.
    /// </summary>
    public Guid FirstUserId { get; set; }

    /// <summary>
    /// Dados do primeiro usuário.
    /// </summary>
    public UserResponse FirstUser { get; set; }

    /// <summary>
    /// Id do segundo usuário.
    /// </summary>
    public Guid SecondUserId { get; set; }

    /// <summary>
    /// Dados do segundo usuário.
    /// </summary>
    public UserResponse SecondUser { get; set; }

    /// <summary>
    /// Mensagens do chat.
    /// </summary>
    public ICollection<ChatMessageResponse> Messages { get; set; }
}
