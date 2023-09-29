using APPLICATION.DOMAIN.CONTRACTS.SERVICES;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION;
using APPLICATION.DOMAIN.DTOS.MAIL.REQUEST.SENDGRID;
using APPLICATION.DOMAIN.DTOS.RESPONSE.BASE;
using APPLICATION.DOMAIN.UTILS.EXTENSIONS;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using Serilog;
using EmailAddress = APPLICATION.DOMAIN.DTOS.MAIL.REQUEST.EmailAddress;

namespace APPLICATION.APPLICATION.SERVICES.MAIL;

/// <summary>
/// Serviço de e-mail do SendGrid.
/// </summary>
public class SendGridMailService : IMailService<SendGridMailRequest, ApiResponse<object>>
{
    private readonly SendGridClient _sendGridClient;

    /// <summary>
    /// ctor.
    /// </summary>
    /// <param name="appsettings"></param>
    public SendGridMailService(
        IOptions<AppSettings> appsettings)
    {
        _sendGridClient = new SendGridClient(apiKey: appsettings.Value.Mail.ApiKey);
    }

    /// <summary>
    /// Envie um único e-mail simples
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="subject"></param>
    /// <param name="plainTextContent"></param>
    /// <param name="htmlContent"></param>
    /// <returns></returns>
    public async Task<ApiResponse<object>> SendSingleMailAsync(
        EmailAddress from, EmailAddress to, string subject, string plainTextContent, string htmlContent)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(SendGridMailService)} - METHOD {nameof(SendSingleMailAsync)}\n");

        try
        {
            return await _sendGridClient.SendEmailAsync(MailHelper.CreateSingleEmail(
                from.ToSendGridEmailAddress(), to.ToSendGridEmailAddress(), subject, plainTextContent, htmlContent)).ContinueWith(
                    async (responseTask) =>
                    {
                        var response = responseTask.Result;

                        return new ApiResponse<object>(
                            response.IsSuccessStatusCode, response.StatusCode,
                                JsonConvert.DeserializeObject(await response.Body.ReadAsStringAsync()));

                    }).Result;
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    /// Envie um único e-mail de template dinâmico.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="templateId"></param>
    /// <param name="dynamicTemplateData"></param>
    /// <returns></returns>
    public async Task<ApiResponse<object>> SendSingleMailWithTemplateAsync(
        EmailAddress from, EmailAddress to, string templateId, object dynamicTemplateData)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(SendGridMailService)} - METHOD {nameof(SendSingleMailWithTemplateAsync)}\n");

        try
        {
            return await _sendGridClient.SendEmailAsync(MailHelper.CreateSingleTemplateEmail(
                from.ToSendGridEmailAddress(), to.ToSendGridEmailAddress(), templateId, dynamicTemplateData)).ContinueWith(
                    async (responseTask) =>
                    {
                        var response = responseTask.Result;

                        Log.Information(
                               $"[LOG INFORMATION] - E-mail para {to.Name}/{to.Email} enviado com {(response.IsSuccessStatusCode ? "sucesso" : "falha")}!\n");

                        return new ApiResponse<object>(
                           response.IsSuccessStatusCode, response.StatusCode,
                                JsonConvert.DeserializeObject(await response.Body.ReadAsStringAsync()));

                    }).Result;
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }
}
