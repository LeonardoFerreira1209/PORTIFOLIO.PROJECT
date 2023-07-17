using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.EVENTS;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.JOBS;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.MAIL;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION;
using APPLICATION.DOMAIN.DTOS.MAIL.REQUEST.SENDGRID;
using APPLICATION.DOMAIN.DTOS.RESPONSE.UTILS;
using APPLICATION.DOMAIN.UTILS.EXTENSIONS;
using APPLICATION.DOMAIN.UTILS.JOBMETHODS;
using APPLICATION.INFRAESTRUTURE.FACTORY.JOBS;
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

    private readonly HangfireJobFactory _hangfireJobFactory = new();
    private readonly IJobsService _jobsService;
    private readonly JobMethods _jobMethods = new(null);
    private readonly IEventRepository _eventRepository;

    /// <summary>
    /// ctor.
    /// </summary>
    /// <param name="appsettings"></param>
    public SendGridMailService(
        IOptions<AppSettings> appsettings, IEventRepository eventRepository)
    {
        _sendGridClient = new SendGridClient(apiKey: appsettings.Value.Mail.ApiKey);
        _jobsService = _hangfireJobFactory.CreateJobService();
        _eventRepository = eventRepository;
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
    public async Task<ApiResponse<object>> SendSingleMailAsync(EmailAddress from, EmailAddress to, string subject, string plainTextContent, string htmlContent)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(SendGridMailService)} - METHOD {nameof(SendSingleMailAsync)}\n");

        try
        {
            return await _sendGridClient.SendEmailAsync(MailHelper.CreateSingleEmail(
                from.ToSendGridEmailAddress(), to.ToSendGridEmailAddress(), subject, plainTextContent, htmlContent)).ContinueWith(async (responseTask) =>
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
    public async Task<ApiResponse<object>> SendSingleMailWithTemplateAsync(EmailAddress from, EmailAddress to, string templateId, object dynamicTemplateData)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(SendGridMailService)} - METHOD {nameof(SendSingleMailWithTemplateAsync)}\n");

        try
        {
            return await _sendGridClient.SendEmailAsync(MailHelper.CreateSingleTemplateEmail(
                from.ToSendGridEmailAddress(), to.ToSendGridEmailAddress(), templateId, dynamicTemplateData)).ContinueWith(async (responseTask) =>
                {
                    var response = responseTask.Result;

                    if (response.IsSuccessStatusCode is false)
                        _eventRepository.CreateAsync(EventExtensions.CreateMailEvent(
                            "ResendMail", "Re-envio de e-mail com falha!", new
                            {
                                from,
                                to,
                                templateId,
                                dynamicTemplateData

                            })).ContinueWith(
                                async (eventTask) =>
                                {
                                    await _eventRepository.SaveChangesAsync();

                                    Log.Warning(
                                        $"Envio de e-mail falhou, evento de re-envio criado com sucesso: {JsonConvert.SerializeObject(eventTask.Result)}!\n");
                                });

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
