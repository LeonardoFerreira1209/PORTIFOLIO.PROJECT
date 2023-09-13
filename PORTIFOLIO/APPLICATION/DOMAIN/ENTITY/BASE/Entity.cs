using APPLICATION.DOMAIN.ENUMS;

namespace APPLICATION.DOMAIN.ENTITY.BASE;

/// <summary>
/// Classe abstrata de entidades.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// ctor com id.
    /// </summary>
    /// <param name="id"></param>
    protected Entity(Guid id) => Id = id;

    /// <summary>
    /// ctor
    /// </summary>
    public Entity()
    {

    }

    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Data de atualização
    /// </summary>
    public DateTime? Updated { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    public virtual Status Status { get; set; }
}
