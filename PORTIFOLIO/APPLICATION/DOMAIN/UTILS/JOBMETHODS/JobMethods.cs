using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;
using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.EVENTS;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.MAIL;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION;
using APPLICATION.DOMAIN.DTOS.MAIL.REQUEST.SENDGRID;
using APPLICATION.DOMAIN.DTOS.RESPONSE.BASE;
using APPLICATION.DOMAIN.ENUMS;
using APPLICATION.DOMAIN.FACTORY.MAIL;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using static APPLICATION.DOMAIN.DTOS.MAIL.ResendMailEventDto;

namespace APPLICATION.DOMAIN.UTILS.JOBMETHODS;

/// <summary>
/// Classe de declaração de Métodos dos JOBS
/// </summary>
public class JobMethods : IJobMethods
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMailService<SendGridMailRequest, ApiResponse<object>> _mailService;

    /// <summary>
    /// 
    /// </summary>
    public JobMethods(
        IEventRepository eventRepository, IUnitOfWork unitOfWork, IOptions<AppSettings> appsettings)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
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
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(JobMethods)} - METHOD {nameof(ResendFailedMailsAsync)}\n");

        await _eventRepository.GetAllAsync(false,
            even => even.Type == EventType.Mail
                && (even.Status == EventStatus.Failed || even.Status == EventStatus.Unprocessed)).ContinueWith(
                    async (taskResult) =>
                    {
                        var events = taskResult.Result.ToList();

                        events.ForEach(async (even) =>
                        {
                            Log.Information($@"[LOG INFORMATION] - 
                                Processando tentativa {even.Retries + 1}, de reenvio de evento {JsonConvert.SerializeObject(even)}, de e-mail de confirmação! \n");

                            ResendConirmationMailEventDto data
                                = JsonConvert.DeserializeObject<ResendConirmationMailEventDto>(even.Data);

                            var response = await _mailService.SendSingleMailWithTemplateAsync(
                                data.From,
                                data.To,
                                data.TemplateId,
                                new
                                {
                                    name = data.DynamicTemplateData.Name,
                                    code = data.DynamicTemplateData.Code,

                                });

                            even.Updated = DateTime.UtcNow;
                            even.Status = response.Sucesso ? EventStatus.Processed : EventStatus.Failed;
                            even.Retries++;
                        });

                        await _eventRepository.BulkUpdateAsync(events).ContinueWith(async (taskResult) =>
                        {
                            _unitOfWork.CommitAsync();
                        });

                    }).Unwrap();
    }
}