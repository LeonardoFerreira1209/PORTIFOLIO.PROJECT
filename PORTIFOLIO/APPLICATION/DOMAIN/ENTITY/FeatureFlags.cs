using APPLICATION.DOMAIN.ENTITY.BASE;

namespace APPLICATION.DOMAIN.ENTITY;

/// <summary>
/// Feature Flags entity
/// </summary>
public class FeatureFlags : Entity
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Está ativa
    /// </summary>
    public bool IsEnabled { get; set; }
}
