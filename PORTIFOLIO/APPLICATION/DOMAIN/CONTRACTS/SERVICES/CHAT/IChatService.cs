using APPLICATION.DOMAIN.ENTITY.CHAT;
using Microsoft.AspNetCore.Mvc;

namespace APPLICATION.DOMAIN.CONTRACTS.SERVICES.USER;

/// <summary>
/// Interface de ChatService
/// </summary>
public interface IChatService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="chatEntity"></param>
    /// <returns></returns>
    Task<ObjectResult> CreateChatAsync(ChatEntity chatEntity);

    /// <summary>
    /// Métodos responsável por recuperar Chats do usuário.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ObjectResult> GetChatsByUserAsync(Guid userId);
}
