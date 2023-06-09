﻿using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.EVENTS;
using APPLICATION.DOMAIN.ENTITY;
using APPLICATION.INFRAESTRUTURE.CONTEXTO;

namespace APPLICATION.INFRAESTRUTURE.REPOSITORY.EVENTS;

public class EventRepository : GenericEntityCoreRepository<Events>, IEventRepository
{
    public EventRepository(Context context) : base(context)
    {

    }
}
