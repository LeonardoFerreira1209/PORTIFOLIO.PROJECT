using APPLICATION.DOMAIN.DTOS.CHAT;
using APPLICATION.DOMAIN.ENTITY.CHAT;
using APPLICATION.DOMAIN.ENUMS;

namespace APPLICATION.DOMAIN.UTILS.EXTENSIONS;

/// <summary>
/// Classe de extensão de chat.
/// </summary>
public static class ChatExtensions
{
    /// <summary>
    /// Método que converte request para entidade de chat.
    /// </summary>
    /// <param name="chatRequest"></param>
    /// <returns></returns>
    public static Chat AsEntity(
        this ChatRequest chatRequest)
        => new()
        {
            FirstUserId = chatRequest.FirstUserId,
            SecondUserId = chatRequest.SecondUserId,
            Created = DateTime.Now,
            Status = Status.Active
        };

    /// <summary>
    /// Método que converte request para entidade de message.
    /// </summary>
    /// <param name="chatMessageRequest"></param>
    /// <returns></returns>
    public static ChatMessage AsEntity(
        this ChatMessageRequest chatMessageRequest)
        => new()
        {
            UserId = chatMessageRequest.UserId,
            ChatId = chatMessageRequest.ChatId,
            Message = chatMessageRequest.Message,
            Created = DateTime.Now,
            Status = Status.Active
        };
}
