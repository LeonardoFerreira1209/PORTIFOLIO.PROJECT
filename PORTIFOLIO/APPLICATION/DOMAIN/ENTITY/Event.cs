using APPLICATION.DOMAIN.ENTITY.BASE;
using APPLICATION.DOMAIN.ENUMS;

namespace APPLICATION.DOMAIN.ENTITY;

/// <summary>
/// Classe de entidade de eventos.
/// </summary>
public class Event : Entity
{
    /// <summary>
    /// Nome do evento.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Descição do evento.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Dados.
    /// </summary>
    public string Data { get; set; }

    /// <summary>
    /// _retry private
    /// </summary>
    private bool _retry;

    /// <summary>
    /// Retentar.
    /// </summary>
    public bool Retry
    {
        get => _retry;

        private set
            => _retry = value && Retries < 3;
    }

    /// <summary>
    /// Tipo de evento.
    /// </summary>
    public EventType Type { get; set; }

    /// <summary>
    /// Retentativas.
    /// </summary>
    public int Retries { get; set; }

    /// <summary>
    /// Status do evento.
    /// </summary>
    public override EventStatus Status { get; set; }
}
