using APPLICATION.DOMAIN.CONTRACTS.SERVICES.JOBS;

namespace APPLICATION.INFRAESTRUTURE.FACTORY.JOBS;

public abstract class JobAbstractFactory
{
    public abstract IJobsService CreateJobService();
}
