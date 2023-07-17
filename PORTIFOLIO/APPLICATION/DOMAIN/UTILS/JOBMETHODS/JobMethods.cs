using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.EVENTS;
using Serilog;

namespace APPLICATION.DOMAIN.UTILS.JOBMETHODS;

public class JobMethods : IJobMethods
{
    private readonly IEventRepository _eventRepository;

    public JobMethods(
        IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task ResendFailedMailsAsync()
    {
        Log.Information("Recurrent job started");
    }
}
