using APPLICATION.DOMAIN.ENTITY.ENTITY;

namespace APPLICATION.DOMAIN.ENTITY.USER;

/// <summary>
/// Entidade de código de confirmação de usuário.
/// </summary>
public class UserCodeEntity : Entity
{
    /// <summary>
    /// Código numérico de confirmação de usuário.
    /// </summary>
    public string NumberCode { get; set; }

    /// <summary>
    /// Id do usuário.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Código de confirmação original.
    /// </summary>
    public string HashCode { get; set; }
}
