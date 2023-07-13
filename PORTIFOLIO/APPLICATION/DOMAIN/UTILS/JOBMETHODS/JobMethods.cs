using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.EVENTS;
using APPLICATION.DOMAIN.ENTITY;
using APPLICATION.DOMAIN.ENUMS;

namespace APPLICATION.DOMAIN.UTILS.JOBMETHODS;

public class JobMethods
{
    private readonly IEventRepository _eventRepository;

    public JobMethods(
        IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task ResendFailedMailsAsync()
    {
        try
        {
            await _eventRepository.CreateAsync(new Events
            {
                Name = "tESTE",
                Status = EventStatus.Unprocessed,
                Retries = 0,
                Description = "Evento de teste",
            });

            await _eventRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {

        }
    }
}
