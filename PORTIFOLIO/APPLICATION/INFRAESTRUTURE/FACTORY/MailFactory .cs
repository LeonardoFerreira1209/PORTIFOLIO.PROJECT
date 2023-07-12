using APPLICATION.APPLICATION.SERVICES.MAIL;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.MAIL;

namespace APPLICATION.INFRAESTRUTURE.FACTORY;

/// <summary>
/// Factory de e-mail padrão do sistema.
/// </summary>
public class MailFactory : MailAbstractFactory
{
    /// <summary>
    /// Método responsável por cruar uma nova instância de IMailService.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public override IMailService<TRequest, TResponse> CreateMailService<TRequest, TResponse>() 
        => new MailService() as IMailService<TRequest, TResponse>;
}
