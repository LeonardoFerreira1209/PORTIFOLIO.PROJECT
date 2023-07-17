using APPLICATION.APPLICATION.SERVICES.MAIL;
using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.EVENTS;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.MAIL;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION;
using Microsoft.Extensions.Options;

namespace APPLICATION.INFRAESTRUTURE.FACTORY.MAIL;

/// <summary>
/// Factory de SendGrid.
/// </summary>
public class SendGridMailFactory : MailAbstractFactory
{
    private readonly IOptions<AppSettings> _appsettings;
    private readonly IEventRepository _eventRepository;

    /// <summary>
    /// ctor.
    /// </summary>
    /// <param name="appsettings"></param>
    public SendGridMailFactory(
        IOptions<AppSettings> appsettings, IEventRepository eventRepository)
    {
        _appsettings = appsettings;
        _eventRepository = eventRepository;
    }

    /// <summary>
    /// Método de criação de uma nova instância de IMailService.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public override IMailService<TRequest, TResponse> CreateMailService<TRequest, TResponse>()
        => new SendGridMailService(_appsettings, _eventRepository) as IMailService<TRequest, TResponse>;
}
