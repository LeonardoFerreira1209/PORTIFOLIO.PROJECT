using System.ComponentModel;

namespace APPLICATION.DOMAIN.ENUMS;

/// <summary>
/// Enum de notificações.
/// </summary>
public enum NotificationType
{
    [Description("Informação")]
    Info = 1,

    [Description("Alerta")]
    Warning = 2,

    [Description("Erro")]
    Danger = 3,

    [Description("Sucesso")]
    Success = 4,

    [Description("Email")]
    Email = 5,
}
