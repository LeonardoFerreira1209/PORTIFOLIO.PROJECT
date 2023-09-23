using APPLICATION.APPLICATION.SERVICES.JOBS;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.JOBS;

namespace APPLICATION.DOMAIN.FACTORY.JOBS;

/// <summary>
/// HangfireJob Factory.
/// </summary>
public class HangfireJobFactory : JobAbstractFactory
{
    /// <summary>
    /// Create a instance o JobService.
    /// </summary>
    /// <returns></returns>
    public override IJobsService CreateJobService()
        => new HangFireJobService();
}
