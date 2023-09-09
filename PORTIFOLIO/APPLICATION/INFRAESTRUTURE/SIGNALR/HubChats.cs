using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.CHAT;
using APPLICATION.DOMAIN.ENTITY.CHAT;
using APPLICATION.DOMAIN.UTILS.GLOBAL;
using Microsoft.AspNetCore.SignalR;

namespace APPLICATION.INFRAESTRUTURE.SIGNALR;

public class HubChats : HubBase
{
    private readonly IChatRepository _chatRepository;

    public HubChats(IChatRepository chatRepository)
        : base(GlobalData.HubChatConnections) 
    { 
        _chatRepository = chatRepository;
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
        await _chatRepository.GetByIdAsync(Guid.Parse(chatId)).ContinueWith(
            taskResult =>
            {
                var chat = taskResult.Result;

                Groups.AddToGroupAsync(Context.ConnectionId, $"chat-{chat.Id}");
            });
    }
}
