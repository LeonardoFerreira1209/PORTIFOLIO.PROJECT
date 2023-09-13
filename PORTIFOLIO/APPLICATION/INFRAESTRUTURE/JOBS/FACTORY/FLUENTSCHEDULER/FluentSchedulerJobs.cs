using APPLICATION.INFRAESTRUTURE.JOBS.INTERFACES.BASE;
using FluentScheduler;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace APPLICATION.INFRAESTRUTURE.JOBS.FACTORY.FLUENTSCHEDULER;

[ExcludeFromCodeCoverage]
public class FluentSchedulerJobs : Registry, IFluentSchedulerJobs
{
    public FluentSchedulerJobs()
    {
        try
        {
            // NonReentrantAsDefault(); Schedule<IResendFailedMailsJob>().ToRunEvery(24).Hours();
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERRO] - Falha na inicialização do Job do Fluent Scheduler. ({exception.Message})\n");
        }
    }
}
