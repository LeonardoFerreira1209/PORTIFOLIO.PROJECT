using APPLICATION.DOMAIN.CONTRACTS.SERVICES;
using APPLICATION.DOMAIN.DTOS.CHAT;
using APPLICATION.DOMAIN.DTOS.RESPONSE.BASE;
using APPLICATION.DOMAIN.DTOS.RESPONSE.CHAT;
using APPLICATION.DOMAIN.UTILS.GLOBAL;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Serilog;

namespace APPLICATION.APPLICATION.SIGNALR;

/// <summary>
/// Hub de chat.
/// </summary>
public class HubChats : HubBase
{
    private readonly IChatService _chatService;
    
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="chatService"></param>
    public HubChats(IChatService chatService)
        : base(GlobalData.HubChatConnections)
    {
        _chatService = chatService;
    }

    /// <summary>
    /// Enviar mensagem para usuário.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="groupName"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task SendMessageToChatAsync(string userId, string chatId, string groupName, string message)
    {
        try
        {
            await _chatService.SendMessageAsync(new ChatMessageRequest
            {
                ChatId = Guid.Parse(chatId),
                Message = message,
                UserId = Guid.Parse(userId)

            }).ContinueWith(async (taskResult) =>
            {
                var apiResponse
                    = (ApiResponse<ChatMessageResponse>)taskResult.Result.Value;

                await Clients
                   .Group(groupName).SendAsync("ReceberMensagem", apiResponse.Dados);

            }).Unwrap();
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    /// Entrar em um grupo.
    /// </summary>
    /// <param name="chatId"></param>
    /// <returns></returns>
    public Task JoinGroup(string chatId)
        => Groups.AddToGroupAsync(Context.ConnectionId, $"chat-{chatId}");
}
