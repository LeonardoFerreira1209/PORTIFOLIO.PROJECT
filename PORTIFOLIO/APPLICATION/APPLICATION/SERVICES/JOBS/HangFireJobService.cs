using APPLICATION.DOMAIN.CONTRACTS.SERVICES.JOBS;
using APPLICATION.DOMAIN.UTILS.JOBMETHODS;
using Hangfire;
using Newtonsoft.Json;
using Serilog;
using System.Linq.Expressions;

namespace APPLICATION.APPLICATION.SERVICES.JOBS;

/// <summary>
/// Serviço de Jobs do Hangfire.
/// </summary>
public class HangFireJobService : IJobsService
{
    /// <summary>
    /// Criar Recurrent Job.
    /// </summary>
    /// <param name="jobId"></param>
    /// <param name="expression"></param>
    /// <param name="cronExpression"></param>
    public void CreateRecurrentJob<T>(
        string jobId, Expression<Action<T>> expression, string cronExpression) where T : IJobMethods
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(HangFireJobService)} - METHOD {nameof(CreateRecurrentJob)}\n");

        try
        {
            RecurringJob.AddOrUpdate(
                 jobId, expression, cronExpression);
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    /// Remover job recorrente.
    /// </summary>
    /// <param name="jobId"></param>
    public void RemoveRecurrentJob(string jobId)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(HangFireJobService)} - METHOD {nameof(RemoveRecurrentJob)}\n");

        try
        {
            RecurringJob
                .RemoveIfExists(jobId);
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    /// Criar queued Jobs.
    /// </summary>
    /// <param name="queueName"></param>
    /// <param name="expression"></param>
    public void CreateQueuedJobs<T>(
        string queueName, Expression<Action<T>> expression) where T : IJobMethods
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(HangFireJobService)} - METHOD {nameof(CreateQueuedJobs)}\n");

        try
        {
            new BackgroundJobClient()
                .Enqueue(expression);
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }
}
