using APPLICATION.APPLICATION.SIGNALR;
using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;
using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.BASE;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES;
using APPLICATION.DOMAIN.DTOS.REQUEST.CHAT;
using APPLICATION.DOMAIN.DTOS.RESPONSE.BASE;
using APPLICATION.DOMAIN.DTOS.RESPONSE.CHAT;
using APPLICATION.DOMAIN.ENTITY.CHAT;
using APPLICATION.DOMAIN.UTILS.EXTENSIONS;
using APPLICATION.DOMAIN.UTILS.GLOBAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Serilog;
using System.Net;

namespace APPLICATION.APPLICATION.SERVICES.CHAT;

/// <summary>
/// Serviço de chat.
/// </summary>
public class ChatSertvice : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<HubChats> _hubChatsContext;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="chatRepository"></param>
    public ChatSertvice(
        IChatRepository chatRepository, 
        IUnitOfWork unitOfWork, IHubContext<HubChats> hubChatsContext)
    {
        _chatRepository = chatRepository;
        _unitOfWork = unitOfWork;
        _hubChatsContext = hubChatsContext;
    }

    /// <summary>
    /// Criar um chat.
    /// </summary>
    /// <param name="chatRequest"></param>
    /// <returns></returns>
    public async Task<ObjectResult> CreateChatAsync(ChatRequest chatRequest)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(ChatSertvice)} - METHOD {nameof(CreateChatAsync)}\n");

        try
        {
           return await RetryPolicy.ExecuteAsync<ObjectResult>(
               async () => await _chatRepository.CreateAsync(chatRequest.AsEntity())
                .ContinueWith(async taskResult =>
                {
                    await _unitOfWork.CommitAsync();

                    var connectionId 
                        = GlobalData.HubChatConnections
                            .FirstOrDefault(con => con.Key.Equals($"direct-{chatRequest.SecondUserId.ToString().ToLower()}")).Value;

                    return await _chatRepository.GetByIdAsync(
                        taskResult.Result.Id, true).ContinueWith(async (taskResult) =>
                        {
                            var chat = taskResult.Result;

                            if(connectionId is not null) 
                                await _hubChatsContext.Clients.Client(connectionId).SendAsync("ReceiveChats", chat);

                            return new OkObjectResult(
                                new ApiResponse<ChatResponse>(
                                    true, HttpStatusCode.Created, chat, new List<DadosNotificacao>  {
                                        new DadosNotificacao("Chat criado com sucesso!")
                                    }));
                        }).Result;

                }).Result, 3);
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    /// Enviar uma mensagem e vincular com o chat.
    /// </summary>
    /// <param name="chatMessageRequest"></param>
    /// <returns></returns>
    public async Task<ObjectResult> SendMessageAsync(ChatMessageRequest chatMessageRequest)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(ChatSertvice)} - METHOD {nameof(SendMessageAsync)}\n");

        try
        {
            var chatMessage = chatMessageRequest.AsEntity();

            return await _chatRepository.CreateMessageAsync(chatMessage)
                .ContinueWith(async taskResult =>
                {
                    await _unitOfWork.CommitAsync();

                    return await _chatRepository.GetMessageByIdAsync(
                        chatMessage.Id).ContinueWith(taskResult =>
                        {
                            var message = taskResult.Result;

                            return new OkObjectResult(
                                new ApiResponse<ChatMessageResponse>(
                                    true, HttpStatusCode.Created, message.ToResponse(), new List<DadosNotificacao>  {
                                            new DadosNotificacao("Mensagem enviada com sucesso!")
                                    }));
                        });
                    
                }).Result;
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    /// Método responsável por retornar os chats de um usuário.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ObjectResult> GetChatsByUserAsync(Guid userId, bool ordered)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(ChatSertvice)} - METHOD {nameof(GetChatsByUserAsync)}\n");

        try
        {
            return await _chatRepository.GetAllAsync(true, 
                chat => chat.FirstUserId.Equals(userId) || chat.SecondUserId.Equals(userId)).ContinueWith(taskResult =>
                {
                    var chats
                        = taskResult.Result;

                    var chatsResponse = ordered ? chats.Select(chat => chat.ToResponse())
                        .OrderByDescending(chat => chat?.Messages?
                            .DefaultIfEmpty(new ChatMessageResponse { Created = DateTime.MinValue })
                                .Max(message => message.Created)).ToList() : chats.Select(chat => chat.ToResponse());

                    return new OkObjectResult(
                        new ApiResponse<ICollection<ChatMessageResponse>>(
                            true, HttpStatusCode.OK, chatsResponse, new List<DadosNotificacao>  {
                                new DadosNotificacao("Chats recuperados com sucesso!")
                            }));
                });
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chatId"></param>
    /// <returns></returns>
    public async Task<ObjectResult> GetChatsByIdAsync(Guid chatId)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(ChatSertvice)} - METHOD {nameof(GetChatsByIdAsync)}\n");

        try
        {
            return await _chatRepository.GetByIdAsync(chatId, true)
                .ContinueWith(taskResult =>
                {
                    var chat 
                        = taskResult.Result;

                    var chatResponse = chat.ToResponse();

                    return new OkObjectResult(
                        new ApiResponse<ChatResponse>(
                            true, HttpStatusCode.OK, chatResponse, new List<DadosNotificacao>  {
                                new DadosNotificacao("Chat recuperado com sucesso!")
                            }));
                });
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    /// Método responsável por retornar dados de um chat.
    /// </summary>
    /// <param name="chatId"></param>
    /// <returns></returns>
    public async Task<ObjectResult> GetByIdAsync(Guid chatId)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(ChatSertvice)} - METHOD {nameof(GetByIdAsync)}\n");

        try
        {
            return await _chatRepository.GetByIdAsync(chatId, true).ContinueWith(taskResult =>
                {
                    var chat
                        = taskResult.Result;

                    return new OkObjectResult(
                        new ApiResponse<Chat>(
                            true, HttpStatusCode.OK, chat.ToResponse(), new List<DadosNotificacao>  {
                                new DadosNotificacao("Chat recuperado com sucesso!")
                            }));
                });
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    /// Método responsável por retornar as mensagens de um chat.
    /// </summary>
    /// <param name="chatId"></param>
    /// <returns></returns>
    public async Task<ObjectResult> GetMessagesByChatAsync(Guid chatId)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(ChatSertvice)} - METHOD {nameof(GetMessagesByChatAsync)}\n");

        try
        {
            return await _chatRepository.GetMessagesByChatAsync(chatId).ContinueWith((taskResult) =>
            {
                var messages
                     = taskResult.Result;

                return new OkObjectResult(
                        new ApiResponse<ICollection<ChatMessageResponse>>(
                            true, HttpStatusCode.OK, messages.Select(message => message.ToResponse()), new List<DadosNotificacao>  {
                                new DadosNotificacao("Chats recuperados com sucesso!")
                            }));
            });
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }

    /// <summary>
    /// Buscar mensagems pelo Id.
    /// </summary>
    /// <param name="chatMessageId"></param>
    /// <returns></returns>
    public async Task<ObjectResult> GetMessageByIdAsync(Guid chatMessageId)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(ChatSertvice)} - METHOD {nameof(GetMessageByIdAsync)}\n");

        try
        {
            return await _chatRepository.GetMessageByIdAsync(chatMessageId).ContinueWith((taskResult) =>
            {
                var message
                     = taskResult.Result;

                return new OkObjectResult(
                        new ApiResponse<ChatMessageResponse>(
                            true, HttpStatusCode.OK, message.ToResponse(), new List<DadosNotificacao>  {
                                new DadosNotificacao("Chat recuperado com sucesso!")
                            }));
            });
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }
}
