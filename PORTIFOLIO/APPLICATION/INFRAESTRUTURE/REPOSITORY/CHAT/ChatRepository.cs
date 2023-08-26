using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.EVENTS;
using APPLICATION.DOMAIN.ENTITY.CHAT;
using APPLICATION.INFRAESTRUTURE.CONTEXTO;

namespace APPLICATION.INFRAESTRUTURE.REPOSITORY.EVENTS;

public class ChatRepository : GenericEntityCoreRepository<ChatEntity>, IChatRepository
{
    public ChatRepository(Context context) : base(context)
    {

    }
}
