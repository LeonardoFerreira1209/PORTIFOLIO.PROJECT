using System.Linq.Expressions;

namespace APPLICATION.DOMAIN.CONTRACTS.SERVICES.JOBS;

public interface IJobsService
{
    void CreateRecurrentJob(string jobId, Expression<Action> method, string cronExpression);
}
