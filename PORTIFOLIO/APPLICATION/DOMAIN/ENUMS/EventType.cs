using System.ComponentModel;

namespace APPLICATION.DOMAIN.ENUMS;

/// <summary>
/// Enum de tipos de eventos.
/// </summary>
public enum EventType
{
    /// <summary>
    /// E-mail.
    /// </summary>
    [Description("Mail")]
    Mail = 1,

    /// <summary>
    /// Mensageria do service bus.
    /// </summary>
    [Description("Bus")]
    Bus = 2,
}
