using APPLICATION.DOMAIN.DTOS.MAIL.REQUEST;

namespace APPLICATION.DOMAIN.CONTRACTS.SERVICES
{
    /// <summary>
    /// Serviço de envio de e-mails usando o SendGrid.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IMailService<TRequest, TResponse>
        where TRequest : MailRequestBase where TResponse : class
    {
        /// <summary>
        /// Envie um único e-mail simples.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="plainTextContent"></param>
        /// <param name="htmlContent"></param>
        /// <returns></returns>
        Task<TResponse> SendSingleMailAsync(
            EmailAddress from, EmailAddress to, string subject, string plainTextContent, string htmlContent);

        /// <summary>
        /// Envie um único e-mail de template dinâmico.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="templateId"></param>
        /// <param name="dynamicTemplateData"></param>
        /// <returns></returns>
        Task<TResponse> SendSingleMailWithTemplateAsync(
            EmailAddress from, EmailAddress to, string templateId, object dynamicTemplateData);
    }
}
