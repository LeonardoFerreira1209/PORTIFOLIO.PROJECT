using APPLICATION.DOMAIN.CONTRACTS.API;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES;
using APPLICATION.DOMAIN.DTOS.REQUEST;
using APPLICATION.DOMAIN.DTOS.REQUEST.CHAT;
using APPLICATION.DOMAIN.DTOS.RESPONSE.BASE;
using APPLICATION.DOMAIN.DTOS.RESPONSE.CHAT;
using APPLICATION.DOMAIN.ENUMS;
using APPLICATION.DOMAIN.UTILS.GLOBAL;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Serilog;
using System.Text.RegularExpressions;

namespace APPLICATION.APPLICATION.SIGNALR;

/// <summary>
/// Hub de chat.
/// </summary>
public class HubChats : HubBase
{
    private readonly IChatService _chatService;
    private readonly IOpenAiExternal _openAiExternal;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="chatService"></param>
    public HubChats(IChatService chatService,
        IOpenAiExternal openAiExternal)
        : base(GlobalData.HubChatConnections)
    {
        _chatService = chatService;
        _openAiExternal = openAiExternal;
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
            var (isCommand, command) = CheckCommand(message);

            await SendToChatAsync(new ChatMessageRequest
            {
                ChatId = Guid.Parse(chatId),
                Message = message,
                UserId = Guid.Parse(userId),
                HasCommand = isCommand,
                Command = command,
                IsChatBot = false
            }, groupName);

            if (isCommand) await SendCommandMessage(userId, chatId, groupName, message, command);
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
    
    /// <summary>
    /// Checa se é um commando.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private static (bool isCommand, string command) CheckCommand(string message)
    {
        string pattern = "(>[\\S]+)[\\s]+([\\s\\S]+)";
        var match = Regex.Match(message, pattern);
        var command = match.Groups[1].Value.ToUpper();
        var isCommand = match.Success;

        return (isCommand, command);
    }

    /// <summary>
    /// Envia uma mensagem baseado no commando.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="chatId"></param>
    /// <param name="groupName"></param>
    /// <param name="message"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    private async Task SendCommandMessage(
        string userId, string chatId, string groupName, string message, string command)
    {
        switch (command)
        {
            case ">GPT":
                await SendQuestionToGptAsync(userId, chatId, groupName, message);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Envia a mensagem para os ecutadores.
    /// </summary>
    /// <param name="chatMessageRequest"></param>
    /// <param name="groupName"></param>
    /// <returns></returns>
    private async Task SendToChatAsync(ChatMessageRequest chatMessageRequest, string groupName)
    {
        await _chatService.SendMessageAsync(chatMessageRequest)
            .ContinueWith(async (taskResult) =>
            {
                var apiResponse
                    = (ApiResponse<ChatMessageResponse>)taskResult.Result.Value;

                await Clients
                   .Group(groupName).SendAsync("ReceberMensagem", apiResponse.Dados);

            }).Unwrap();
    }

    /// <summary>
    /// Envia uma pergunta para 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="chatId"></param>
    /// <param name="groupName"></param>
    /// <param name="messageSubstring"></param>
    /// <returns></returns>
    private async Task SendQuestionToGptAsync(
        string userId, string chatId, string groupName, string messageSubstring)
    {
        var request
            = new OpenAiCompletionsRequest
            {
                Model = "gpt-3.5-turbo",
                Messages = new List<OpenAiCompletionsMessagesRequest> {
                    new OpenAiCompletionsMessagesRequest {
                        Content = messageSubstring,
                        Role = nameof(CompletionsRoles.User).ToLower()
                    }
                }
            };

        await _openAiExternal.Completions(
            request).ContinueWith(async (taskResult) =>
            {
                var response = taskResult.Result;

                await SendToChatAsync(new ChatMessageRequest
                {
                    ChatId = Guid.Parse(chatId),
                    Message = $"GPT > {response.Choices.First().openAiCompletionsMessageResponse.Content}",
                    UserId = Guid.Parse(userId),
                    Command = "GPT >",
                    HasCommand = true,
                    IsChatBot = true
                }, groupName);

            }).Result;
    }
}
