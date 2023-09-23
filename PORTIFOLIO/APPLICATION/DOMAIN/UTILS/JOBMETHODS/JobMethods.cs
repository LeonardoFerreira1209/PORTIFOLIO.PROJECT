using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;
using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.EVENTS;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.MAIL;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION;
using APPLICATION.DOMAIN.DTOS.MAIL;
using APPLICATION.DOMAIN.DTOS.MAIL.REQUEST.SENDGRID;
using APPLICATION.DOMAIN.DTOS.RESPONSE.BASE;
using APPLICATION.DOMAIN.ENUMS;
using APPLICATION.DOMAIN.FACTORY.MAIL;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace APPLICATION.DOMAIN.UTILS.JOBMETHODS;

/// <summary>
/// 
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
        Log.Information($"[LOG INFORMATION] - Executando JOB: {nameof(ResendFailedMailsAsync)}!\n");

        await _eventRepository.GetAllAsync(false,
            even => even.Type == EventType.Mail
                && (even.Status == EventStatus.Failed || even.Status == EventStatus.Unprocessed)).ContinueWith(
                    (taskResult) =>
                    {
                        taskResult.Result?.ToList().ForEach(async (even) =>
                        {
                            var data = JsonConvert.DeserializeObject<EventMailDetailsConfirmEmailDto>(even.Data);

                           var response = await _mailService.SendSingleMailWithTemplateAsync(
                                data.From,
                                data.To,
                                data.TemplateId,
                                data.DynamicTemplateData);

                            if (response.Sucesso)
                            {
                                even.Status = EventStatus.Processed;
                                even.Updated = DateTime.UtcNow;

                                await _eventRepository.UpdateAsync(even);
                            }
                            else
                            {
                                even.Status = EventStatus.Unprocessed;
                                even.Updated = DateTime.UtcNow;
                                even.Retries = even.Retries++;

                                await _eventRepository.UpdateAsync(even);
                            }

                            await _unitOfWork.CommitAsync();

                            Log.Information($"[LOG INFORMATION] - Evento: {JsonConvert.SerializeObject(even)}, processado com {(response.Sucesso ? "sucesso" : "falha")}!\n");
                        });

                        Log.Information($"[LOG INFORMATION] - JOB: {nameof(ResendFailedMailsAsync)}, processado com sucesso!\n");
                    });
    }
}