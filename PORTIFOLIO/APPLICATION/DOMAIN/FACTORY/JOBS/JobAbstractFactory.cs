using APPLICATION.DOMAIN.CONTRACTS.SERVICES;

namespace APPLICATION.DOMAIN.FACTORY.JOBS;

public abstract class JobAbstractFactory
{
    public abstract IJobsService CreateJobService();
}
