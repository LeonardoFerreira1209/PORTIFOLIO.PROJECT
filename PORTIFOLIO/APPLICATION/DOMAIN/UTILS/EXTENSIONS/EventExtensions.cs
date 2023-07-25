using APPLICATION.DOMAIN.ENTITY;
using APPLICATION.DOMAIN.ENUMS;
using Newtonsoft.Json;

namespace APPLICATION.DOMAIN.UTILS.EXTENSIONS;

/// <summary>
/// Classe de extensão de eventos.
/// </summary>
public static class EventExtensions
{
    /// <summary>
    /// Método responsável por riar evento de e-mail.
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    public static EventEntity CreateMailEvent(
        string eventName, string description, object data) => new()
        {
            Name = eventName,
            Description = description,
            Retries = 0,
            Data = JsonConvert.SerializeObject(data),
            Status = EventStatus.Unprocessed,
            Type = EventType.Mail,
            Created = DateTime.Now,
        };
}
