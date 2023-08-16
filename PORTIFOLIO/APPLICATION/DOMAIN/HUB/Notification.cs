using APPLICATION.DOMAIN.ENUMS;
using APPLICATION.DOMAIN.UTILS.EXTENSIONS;

namespace APPLICATION.DOMAIN.HUB;

/// <summary>
/// Classe de dados de notificação
/// </summary>
public class Notification
{
    /// <summary>
    /// ctor
    /// </summary>
    public Notification()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Id da notficação.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Mensagem da notificação.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Tipo da notificação.
    /// </summary>
    public NotificationType Type { get; set; }

    /// <summary>
    /// Descrição do tipo da notificação.
    /// </summary>
    public string TypeDescription { get => Type.ObterDescricao(); }

    /// <summary>
    /// Data de criação.
    /// </summary>
    public DateTime CreatedDate { get; set; }
}
