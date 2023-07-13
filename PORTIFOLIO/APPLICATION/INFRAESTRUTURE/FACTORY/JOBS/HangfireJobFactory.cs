using APPLICATION.APPLICATION.SERVICES.JOBS;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.JOBS;

namespace APPLICATION.INFRAESTRUTURE.FACTORY.JOBS;

public class HangfireJobFactory : JobAbstractFactory
{
    public override IJobsService CreateJobService()
        => new HangFireJobService();
}
