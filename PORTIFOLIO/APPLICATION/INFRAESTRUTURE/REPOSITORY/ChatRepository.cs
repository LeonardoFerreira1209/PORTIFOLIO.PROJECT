using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;
using APPLICATION.DOMAIN.ENTITY.CHAT;
using APPLICATION.INFRAESTRUTURE.CONTEXTO;
using APPLICATION.INFRAESTRUTURE.REPOSITORY.BASE;
using Microsoft.EntityFrameworkCore;

namespace APPLICATION.INFRAESTRUTURE.REPOSITORY;

/// <summary>
/// Repositório de chat.
/// </summary>
public class ChatRepository : GenericEntityCoreRepository<Chat>, IChatRepository
{
    private readonly Context _context;
    private readonly LazyLoadingContext _lazyLoadingContext;

    /// <summary>
    /// ctor.
    /// </summary>
    /// <param name="context"></param>
    public ChatRepository(LazyLoadingContext lazyLoadingContext,
        Context context) : base(context, lazyLoadingContext)
    {
        _context = context;
        _lazyLoadingContext = lazyLoadingContext;
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
    public async Task<ICollection<ChatMessage>> GetMessagesByChatAsync(Guid chatId)
        => await _lazyLoadingContext
            .ChatMessages.Where(message => message.ChatId.Equals(chatId)).ToListAsync();

    /// <summary>
    /// Buscar mensagems pelo Id.
    /// </summary>
    /// <param name="chatMessageId"></param>
    /// <returns></returns>
    public async Task<ChatMessage> GetMessageByIdAsync(Guid chatMessageId)
        => await Task.FromResult(_lazyLoadingContext
            .ChatMessages.FirstOrDefault(chatMessage => chatMessage.Id.Equals(chatMessageId)));
}
