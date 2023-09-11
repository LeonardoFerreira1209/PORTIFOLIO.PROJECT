using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;
using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.CHAT;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.CHAT;
using APPLICATION.DOMAIN.DTOS.CHAT;
using APPLICATION.DOMAIN.DTOS.RESPONSE.CHAT;
using APPLICATION.DOMAIN.DTOS.RESPONSE.UTILS;
using APPLICATION.DOMAIN.ENTITY.CHAT;
using APPLICATION.DOMAIN.UTILS.EXTENSIONS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="chatRepository"></param>
    public ChatSertvice(
        IChatRepository chatRepository, IUnitOfWork unitOfWork)
    {
        _chatRepository = chatRepository;
        _unitOfWork = unitOfWork;
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
            return await _chatRepository.CreateAsync(chatRequest.AsEntity())
                .ContinueWith(async astaskResult =>
                 {
                     await _unitOfWork.CommitAsync();

                     return new OkObjectResult(
                         new ApiResponse<ChatResponse>(
                             true, HttpStatusCode.Created, null, new List<DadosNotificacao>  {
                                    new DadosNotificacao("Chat criado com sucesso!")
                             }));
                 }).Result;
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
                .ContinueWith(async astaskResult =>
                {
                    await _unitOfWork.CommitAsync();

                    return new OkObjectResult(
                        new ApiResponse<ChatMessageResponse>(
                            true, HttpStatusCode.Created, chatMessage, new List<DadosNotificacao>  {
                                    new DadosNotificacao("Mensagem enviada com sucesso!")
                            }));
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
    public async Task<ObjectResult> GetChatsByUserAsync(Guid userId)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(ChatSertvice)} - METHOD {nameof(GetChatsByUserAsync)}\n");

        try
        {
            return await _chatRepository.GetAllAsync(
                chat => chat.FirstUserId.Equals(userId) || chat.SecondUserId.Equals(userId)).ContinueWith(taskResult =>
                {
                    var chats
                        = taskResult.Result
                            .Include(u => u.FirstUser)
                            .Include(u => u.SecondUser)
                            .Include(x => x.Messages).ToList();

                    return new OkObjectResult(
                        new ApiResponse<ICollection<ChatMessage>>(
                            true, HttpStatusCode.OK, chats.Select(chat => chat.ToResponse()), new List<DadosNotificacao>  {
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
