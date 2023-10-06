using System.ComponentModel;

namespace APPLICATION.DOMAIN.ENUMS;

/// <summary>
/// Enum de roles. 
/// </summary>
public enum CompletionsRoles
{
    /// <summary>
    /// Mensagem do sistema.
    /// </summary>
    [Description("system")]
    System = 1,

    /// <summary>
    /// Mensagem do usuário.
    /// </summary>
    [Description("user")]
    User = 2,
    
    /// <summary>
    /// Mensagem da IA.
    /// </summary>
    [Description("assistant")]
    Assistant = 3,
}
