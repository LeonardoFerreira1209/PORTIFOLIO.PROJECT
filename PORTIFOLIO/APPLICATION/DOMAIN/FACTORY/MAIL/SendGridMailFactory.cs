using APPLICATION.APPLICATION.SERVICES.MAIL;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.MAIL;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION;
using Microsoft.Extensions.Options;

namespace APPLICATION.DOMAIN.FACTORY.MAIL;

/// <summary>
/// Factory de SendGrid.
/// </summary>
public class SendGridMailFactory : MailAbstractFactory
{
    private readonly IOptions<AppSettings> _appsettings;

    /// <summary>
    /// ctor.
    /// </summary>
    /// <param name="appsettings"></param>
    public SendGridMailFactory(
        IOptions<AppSettings> appsettings)
    {
        _appsettings = appsettings;
    }

    /// <summary>
    /// Método de criação de uma nova instância de IMailService.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public override IMailService<TRequest, TResponse> CreateMailService<TRequest, TResponse>()
        => new SendGridMailService(_appsettings) as IMailService<TRequest, TResponse>;
}
