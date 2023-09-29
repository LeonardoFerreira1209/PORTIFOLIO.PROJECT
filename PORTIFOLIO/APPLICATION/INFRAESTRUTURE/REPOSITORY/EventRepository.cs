using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;
using APPLICATION.DOMAIN.ENTITY;
using APPLICATION.INFRAESTRUTURE.CONTEXTO;
using APPLICATION.INFRAESTRUTURE.REPOSITORY.BASE;

namespace APPLICATION.INFRAESTRUTURE.REPOSITORY;

public class EventRepository : GenericEntityCoreRepository<Event>, IEventRepository
{
    public EventRepository(LazyLoadingContext lazyLoadingContext, Context context) : base(context, lazyLoadingContext) { }
}
