using APPLICATION.INFRAESTRUTURE.JOBS.INTERFACES.BASE;
using APPLICATION.INFRAESTRUTURE.JOBS.INTERFACES.RECURRENT;
using Hangfire;
using Serilog;

namespace APPLICATION.INFRAESTRUTURE.JOBS.FACTORY.HANGFIRE;

/// <summary>
/// Classe de Jobs Hangfire.
/// </summary>
public class HangfireJobs : IHangfireJobs
{
    private readonly IRecurringJobManager _recurringJobManager;

    /// <summary>
    /// ctor.
    /// </summary>
    /// <param name="recurringJobManager"></param>
    public HangfireJobs(
        IRecurringJobManager recurringJobManager)
    {
        _recurringJobManager = recurringJobManager;
    }

    /// <summary>
    /// Registrar Jobs
    /// </summary>
    public void RegistrarJobs()
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(HangfireJobs)} - METHOD {nameof(RegistrarJobs)}\n");

        try
        {
            _recurringJobManager.AddOrUpdate<IResendFailedMailsJob>(
                "processa-resend-failed-mails", job => job.Execute(), "*/15 * * * *");
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERRO] - Falha na inicialização do Job do Hangfire. ({exception.Message})\n");
        }
    }
}
