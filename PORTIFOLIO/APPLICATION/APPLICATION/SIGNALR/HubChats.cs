using APPLICATION.DOMAIN.CONTRACTS.SERVICES.CHAT;
using APPLICATION.DOMAIN.DTOS.RESPONSE.UTILS;
using APPLICATION.DOMAIN.ENTITY.CHAT;
using APPLICATION.DOMAIN.UTILS.GLOBAL;
using Microsoft.AspNetCore.SignalR;

namespace APPLICATION.APPLICATION.SIGNALR;

public class HubChats : HubBase
{
    private readonly IChatService _chatService;

    public HubChats(IChatService chatService)
        : base(GlobalData.HubChatConnections)
    {
        _chatService = chatService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="groupName"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task SendMessageToChatAsync(string userId, string groupName, string message)
    {
        await Clients
            .Group(groupName).SendAsync("ReceberMensagem", new ChatMessage
            {
                Message = message,
                UserId = Guid.Parse(userId)
            });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chatId"></param>
    /// <returns></returns>
    public async Task JoinGroup(string chatId)
    {
        await _chatService.GetByIdAsync(Guid.Parse(chatId)).ContinueWith(
            taskResult =>
            {
                var apiResponse = taskResult.Result;

                //Groups.AddToGroupAsync(Context.ConnectionId, $"chat-{chat.Id}");
            });
    }
}
