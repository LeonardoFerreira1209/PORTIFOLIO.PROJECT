using APPLICATION.ENUMS;

namespace APPLICATION.DOMAIN.ENTITY.ENTITY;

public abstract class Entity
{
    protected Entity(Guid id) => Id = id;

    protected Entity() {

    }

    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTime Created { get; protected set; }

    /// <summary>
    /// Data de atualização
    /// </summary>
    public DateTime? Updated { get; protected set; }

    /// <summary>
    /// Status
    /// </summary>
    public Status Status { get; protected set; }
}
