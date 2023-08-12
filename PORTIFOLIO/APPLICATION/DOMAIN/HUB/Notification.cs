using APPLICATION.DOMAIN.ENUMS;

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
    /// Data de criação.
    /// </summary>
    public DateTime CreatedDate { get; set; }
}
