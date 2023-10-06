using APPLICATION.DOMAIN.CONTRACTS.SERVICES;
using APPLICATION.DOMAIN.DTOS.REQUEST.MAIL.BASE;
using APPLICATION.DOMAIN.DTOS.REQUEST.MAIL.SENDGRID;
using Refit;

namespace APPLICATION.APPLICATION.SERVICES.MAIL;

public class MailService : IMailService<SendGridMailRequest, ApiResponse<object>>
{
    public Task<ApiResponse<object>> SendSingleMailAsync(EmailAddress from, EmailAddress to, string subject, string plainTextContent, string htmlContent)
    {
        throw new NotImplementedException();
    }

    Task<ApiResponse<object>> IMailService<SendGridMailRequest, ApiResponse<object>>.SendSingleMailWithTemplateAsync(EmailAddress from, EmailAddress to, string templateId, object dynamicTemplateData)
    {
        throw new NotImplementedException();
    }
}
