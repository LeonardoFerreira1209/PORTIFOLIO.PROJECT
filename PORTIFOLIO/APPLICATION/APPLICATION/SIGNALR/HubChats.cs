using APPLICATION.DOMAIN.CONTRACTS.API;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES;
using APPLICATION.DOMAIN.DTOS.REQUEST;
using APPLICATION.DOMAIN.DTOS.REQUEST.CHAT;
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
    public async Task SendMessageToChatAsync(string userId, string chatId, string groupName, ChatHubMessageRequest message)
    {
        try
        {
            if (!string.IsNullOrEmpty(message.Message))
            {
                var (isCommand, command) = CheckCommand(message.Message);

                await SendToChatAsync(new ChatMessageRequest
                {
                    ChatId = Guid.Parse(chatId),
                    Message = message.Message,
                    UserId = Guid.Parse(userId),
                    HasCommand = isCommand,
                    Command = command,
                    IsChatBot = false
                }, groupName);

                if (isCommand) await SendCommandMessage(userId, chatId, groupName, message.Message, command);
            }

            if(message.File is not null)
            {
                await SendToChatAsync(new ChatMessageRequest
                {
                    ChatId= Guid.Parse(chatId),
                    UserId= Guid.Parse(userId),
                    Images = new[] { message.File }.ToList(),
                    IsImage = true
                }, groupName);
            }
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
            case ">DALLE":
                await SendPromptToDalleGenerationImageAsync(userId, chatId, groupName, message);
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
                    = (DOMAIN.DTOS.RESPONSE.BASE.ApiResponse<ChatMessageResponse>)taskResult.Result.Value;

                await Clients
                   .Group(groupName).SendAsync("ReceberMensagem", apiResponse.Dados);

            }).Result;
    }

    /// <summary>
    /// Envia uma pergunta para o GPT.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="chatId"></param>
    /// <param name="groupName"></param>
    /// <param name="messageSubstring"></param>
    /// <returns></returns>
    private async Task SendQuestionToGptAsync(
        string userId, string chatId, string groupName, string messageSubstring)
    {
        try
        {
            var request
                = new OpenAiCompletionsRequest
                {
                    Model = "gpt-4",
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
        catch
        {
            await SendToChatAsync(new ChatMessageRequest
            {
                ChatId = Guid.Parse(chatId),
                UserId = Guid.Parse(userId),
                Command = "GPT >",
                HasCommand = true,
                IsChatBot = true,
                IsImage = false,
                Message = "GPT > Falha ao gerar resposta, tente novamente!",

            }, groupName);
        }
    }

    /// <summary>
    /// Envia um prompt para o DALLE gerar uma imagem.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="chatId"></param>
    /// <param name="groupName"></param>
    /// <param name="messageSubstring"></param>
    /// <returns></returns>
    private async Task SendPromptToDalleGenerationImageAsync(
        string userId, string chatId, string groupName, string messageSubstring)
    {
        try
        {
            var request
                = new OpenAiImagesGenerationRequest
                {
                    Model = "dall-e-3",
                    Prompt = messageSubstring,
                    N = 1,
                    Size = "1024x1024"
                };

            await _openAiExternal.ImageGeneration(
                request).ContinueWith((taskResult) =>
                {
                    var response = taskResult.Result;

                    response.Data.ForEach(async (data) =>
                    {
                        await SendToChatAsync(new ChatMessageRequest
                        {
                            ChatId = Guid.Parse(chatId),
                            UserId = Guid.Parse(userId),
                            Command = "DALLE >",
                            HasCommand = true,
                            IsChatBot = true,
                            IsImage = true,
                            Message = data.url
                        }, groupName);
                    });
                });
        }
        catch
        {
            await SendToChatAsync(new ChatMessageRequest
            {
                ChatId = Guid.Parse(chatId),
                UserId = Guid.Parse(userId),
                Command = "DALLE >",
                HasCommand = true,
                IsChatBot = true,
                IsImage = false,
                Message = "DALLE > Falha ao gerar resposta, tente novamente!",

            }, groupName);
        }
    }
}
