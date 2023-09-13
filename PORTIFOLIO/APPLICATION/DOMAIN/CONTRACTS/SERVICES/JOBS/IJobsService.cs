using APPLICATION.DOMAIN.UTILS.JOBMETHODS;
using System.Linq.Expressions;

namespace APPLICATION.DOMAIN.CONTRACTS.SERVICES.JOBS;

/// <summary>
/// Interface de serviço de Jobs.
/// </summary>
public interface IJobsService
{
    /// <summary>
    /// Método de criação de Jobs Recorrentes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="jobId"></param>
    /// <param name="expression"></param>
    /// <param name="cronExpression"></param>
    void CreateRecurrentJob<T>(string jobId, Expression<Action<T>> expression, string cronExpression)
        where T : IJobMethods;

    /// <summary>
    /// Método de criação de jobs para fila.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="jobId"></param>
    /// <param name="expression"></param>
    void CreateQueuedJobs<T>(string jobId, Expression<Action<T>> expression)
        where T : IJobMethods;

    /// <summary>
    /// Método de pausa de Jobs Recorrentes.
    /// </summary>
    /// <param name="jobId"></param>
    void RemoveRecurrentJob(string jobId);
}
