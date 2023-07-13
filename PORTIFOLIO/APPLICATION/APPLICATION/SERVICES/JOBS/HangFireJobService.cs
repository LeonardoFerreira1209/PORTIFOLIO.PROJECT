using APPLICATION.DOMAIN.CONTRACTS.SERVICES.JOBS;
using Hangfire;
using System.Linq.Expressions;

namespace APPLICATION.APPLICATION.SERVICES.JOBS;

public class HangFireJobService : IJobsService
{
    public HangFireJobService()
    {

    }

    public void CreateRecurrentJob(
        string jobId, Expression<Action> method, string cronExpression)
    {
        try
        {
            RecurringJob.AddOrUpdate(
                 jobId, method, cronExpression);
        }
        catch (Exception ex)
        {

        }
    }
}
