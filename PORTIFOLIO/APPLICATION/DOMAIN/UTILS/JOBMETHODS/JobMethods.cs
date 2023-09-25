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
    /// ctor
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
    /// Método de execução de eventos reenvio de e-mails;
    /// </summary>
    /// <returns></returns>
    public async Task ResendFailedMailsAsync()
    {
        Log.Information($"[LOG INFORMATION] - JOB: Executando {nameof(ResendFailedMailsAsync)}!\n");

        await _eventRepository.GetAllAsync(false,
            even => even.Type == EventType.Mail
                && (even.Status == EventStatus.Failed || even.Status == EventStatus.Unprocessed)).ContinueWith(
                    async (taskResult) =>
                    {
                        var events = taskResult.Result.ToList();

                        Log.Information($"[LOG INFORMATION] - Forma encontrados o total de {events.Count} evento(s).\n");

                        events.ForEach(async (even) =>
                        {
                            Log.Information($"[LOG INFORMATION] - Processando tentativa {even.Retries + 1}, de reenvio de evento {JsonConvert.SerializeObject(even)}, de e-mail de confirmação! \n");

                            ResendConirmationMailEventDto data
                                = JsonConvert.DeserializeObject<ResendConirmationMailEventDto>(even.Data);

                            var response = await _mailService.SendSingleMailWithTemplateAsync(
                                 data.From,
                                 data.To,
                                 data.TemplateId,
                                 new { name = data.DynamicTemplateData.Name, code = data.DynamicTemplateData.Code });

                            even.Status = response.Sucesso ? EventStatus.Processed : EventStatus.Failed;
                            even.Updated = DateTime.Now;
                            even.Retries++;
                        });

                        if (events.Any())
                            await _eventRepository.BulkUpdateAsync(events).ContinueWith(async (taskResult) =>
                            {
                                Log.Information($"[LOG INFORMATION] - Total de {events.Count} evento(s) atualizados com sucesso!\n");

                                await _unitOfWork.CommitAsync();

                            }).Unwrap();

                    }).Unwrap();

        Log.Information($"[LOG INFORMATION] - JOB: {nameof(ResendFailedMailsAsync)}, executado com sucesso!\n");
    }
}