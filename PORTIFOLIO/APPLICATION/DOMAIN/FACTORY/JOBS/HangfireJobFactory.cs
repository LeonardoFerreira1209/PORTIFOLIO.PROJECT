using APPLICATION.APPLICATION.SERVICES.JOBS;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.JOBS;

namespace APPLICATION.DOMAIN.FACTORY.JOBS;

public class HangfireJobFactory : JobAbstractFactory
{
    public override IJobsService CreateJobService()
        => new HangFireJobService();
}
