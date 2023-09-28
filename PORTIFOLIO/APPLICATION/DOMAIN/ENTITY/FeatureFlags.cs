using APPLICATION.DOMAIN.ENTITY.BASE;

namespace APPLICATION.DOMAIN.ENTITY;

/// <summary>
/// Feature Flags entity
/// </summary>
public class FeatureFlags : Entity
{
    /// <summary>
    /// Nome
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Está ativa
    /// </summary>
    public bool IsEnabled { get; set; }
}
