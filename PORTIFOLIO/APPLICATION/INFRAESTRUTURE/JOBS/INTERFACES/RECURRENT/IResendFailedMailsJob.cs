using FluentScheduler;

namespace APPLICATION.INFRAESTRUTURE.JOBS.INTERFACES.RECURRENT;

/// <summary>
/// Interface de ResendFailedMailsJob
/// </summary>
public interface IResendFailedMailsJob : IJob
{
    /// <summary>
    /// Método de reenvio de e-mails com falha.
    /// </summary>
    /// <returns></returns>
    Task ResendFailedMailsJob();
}
