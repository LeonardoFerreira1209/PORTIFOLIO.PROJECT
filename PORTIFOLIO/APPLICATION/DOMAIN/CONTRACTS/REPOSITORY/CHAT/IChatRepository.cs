using APPLICATION.DOMAIN.ENTITY.CHAT;

namespace APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.CHAT;

/// <summary>
/// Interface de Repositório de chat.
/// </summary>
public interface IChatRepository : IGenerictEntityCoreRepository<Chat>
{
    /// <summary>
    /// Criar mensagem.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task<ChatMessage> CreateMessageAsync(ChatMessage message);
}
