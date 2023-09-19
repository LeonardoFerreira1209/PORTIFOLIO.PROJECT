namespace APPLICATION.DOMAIN.ENTITY;

/// <summary>
/// Feature Flags entity
/// </summary>
public class FeatureFlags
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Está ativa
    /// </summary>
    public bool IsEnabled { get; set; }
}
