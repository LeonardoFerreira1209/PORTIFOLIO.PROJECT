using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.EVENTS;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.MAIL;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION;
using APPLICATION.DOMAIN.DTOS.MAIL.REQUEST;
using APPLICATION.DOMAIN.DTOS.MAIL.REQUEST.SENDGRID;
using APPLICATION.DOMAIN.DTOS.RESPONSE.BASE;
using APPLICATION.DOMAIN.ENUMS;
using APPLICATION.DOMAIN.FACTORY.MAIL;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace APPLICATION.DOMAIN.UTILS.JOBMETHODS;

/// <summary>
/// 
/// </summary>
public class JobMethods : IJobMethods
{
    private readonly IEventRepository _eventRepository;
    private readonly IMailService<SendGridMailRequest, ApiResponse<object>> _mailService;

    /// <summary>
    /// 
    /// </summary>
    public JobMethods(
        IEventRepository eventRepository, IOptions<AppSettings> appsettings)
    {
        _eventRepository = eventRepository;
        SendGridMailFactory sendGridMailFactory = new(appsettings);
        _mailService =
            sendGridMailFactory.CreateMailService<SendGridMailRequest, ApiResponse<object>>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task ResendFailedMailsAsync()
    {
        await _eventRepository.GetAllAsync(false,
            even => even.Type == EventType.Mail
                && (even.Status == EventStatus.Failed || even.Status == EventStatus.Unprocessed)).ContinueWith(
                    async (taskResult) =>
                    {
                        taskResult.Result?.ToList().ForEach(async (even) =>
                        {
                            dynamic data = JsonConvert.DeserializeObject<dynamic>(even.Data);

                            var a = (EmailAddress)data.From;

                            await _mailService.SendSingleMailWithTemplateAsync(
                                a,
                                data.To,
                                data.TemplateId,
                                data.DynamicTemplateData);
                        });
                    });
    }
}