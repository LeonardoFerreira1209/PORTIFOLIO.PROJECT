using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.CHAT;
using APPLICATION.DOMAIN.ENTITY.CHAT;
using APPLICATION.INFRAESTRUTURE.CONTEXTO;

namespace APPLICATION.INFRAESTRUTURE.REPOSITORY.CHAT;

/// <summary>
/// Repositório de chat.
/// </summary>
public class ChatRepository : GenericEntityCoreRepository<Chat>, IChatRepository
{
    private readonly Context _context;

    /// <summary>
    /// ctor.
    /// </summary>
    /// <param name="context"></param>
    public ChatRepository(
        Context context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Adicionar mensagem.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task<ChatMessage> CreateMessageAsync(
        ChatMessage message)
    {
        await _context
            .ChatMessages.AddAsync(message);

        return message;
    }

    /// <summary>
    /// Buscar mensagems pelo Id do chat.
    /// </summary>
    /// <param name="chatId"></param>
    /// <returns></returns>
    public async Task<IQueryable<ChatMessage>> GetMessagesByChatAsync(
    Guid chatId) 
        => await Task.FromResult(_context
            .ChatMessages.Where(message => message.ChatId.Equals(chatId)).AsQueryable());
}
