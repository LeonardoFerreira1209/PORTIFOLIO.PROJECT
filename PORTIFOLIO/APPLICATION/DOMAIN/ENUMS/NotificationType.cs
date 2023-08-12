using System.ComponentModel;

namespace APPLICATION.DOMAIN.ENUMS;

public enum NotificationType
{
    [Description("Alerta")]
    Warning = 1,

    [Description("Erro")]
    Danger = 2,

    [Description("Informação")]
    Info = 3
}
