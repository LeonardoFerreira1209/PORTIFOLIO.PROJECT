using APPLICATION.DOMAIN.CONTRACTS.SERVICES.MAIL;
using APPLICATION.DOMAIN.DTOS.MAIL;
using APPLICATION.DOMAIN.DTOS.MAIL.BASE;

namespace APPLICATION.APPLICATION.SERVICES.MAIL;

public class MailService : IMailService<SendGridMailRequest, MailResponseBase>
{
    public Task SendSingleMailWithTemplateAsync(EmailAddress from, EmailAddress to, string templateId, object dynamicTemplateData)
    {
        throw new NotImplementedException();
    }

    Task IMailService<SendGridMailRequest, MailResponseBase>.SendSingleMailAsync(EmailAddress from, EmailAddress to, string subject, string plainTextContent, string htmlContent)
    {
        throw new NotImplementedException();
    }
}
