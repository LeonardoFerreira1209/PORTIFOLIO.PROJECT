using APPLICATION.DOMAIN.DTOS.REQUEST.CHAT;
using APPLICATION.DOMAIN.DTOS.RESPONSE.CHAT;
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
            Created = DateTime.UtcNow,
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
            Created = DateTime.UtcNow,
            Status = Status.Active,
            Command = chatMessageRequest.Command,
            HasCommand = chatMessageRequest.HasCommand,
            IsChatBot = chatMessageRequest.IsChatBot,
            IsImage = chatMessageRequest.IsImage,
        };

    /// <summary>
    ///  Método que converte entity para response de message.
    /// </summary>
    /// <param name="chat"></param>
    /// <returns></returns>
    public static ChatResponse ToResponse(
        this Chat chat)
        => new()
        {
            Id = chat.Id,
            Updated = chat.Updated?.ToLocalTime(),
            Created = chat.Created.ToLocalTime(),
            Status = chat.Status,
            FirstUserId = chat.FirstUserId,
            SecondUserId = chat.SecondUserId,
            FirstUser = chat.FirstUser?.ToResponse(),
            SecondUser = chat.SecondUser?.ToResponse(),
            Messages = chat.Messages?.Select(m => m.ToResponse()).ToList(),
        };

    /// <summary>
    /// Método que converte entity para response de chatMessage.
    /// </summary>
    /// <param name="chatMessage"></param>
    /// <returns></returns>
    public static ChatMessageResponse ToResponse(
        this ChatMessage chatMessage)
        => new()
        {
            Id = chatMessage.Id,
            Message = chatMessage.Message,
            Created = chatMessage.Created.ToLocalTime(),
            ChatId = chatMessage.ChatId,
            Status = chatMessage.Status,
            Updated = chatMessage.Updated?.ToLocalTime(),
            UserId = chatMessage.UserId,
            UserToSendMessage = chatMessage.UserToSendMessage?.ToResponse(),
            Command = chatMessage.Command,
            HasCommand = chatMessage.HasCommand,
            IsChatBot = chatMessage.IsChatBot,
            IsImage = chatMessage.IsImage,
            FileId = chatMessage?.FileId,
            File = chatMessage.File?.ToResponse()
        };
}
