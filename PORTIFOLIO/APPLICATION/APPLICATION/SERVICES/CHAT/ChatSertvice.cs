using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;
using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.EVENTS;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.USER;
using APPLICATION.DOMAIN.DTOS.RESPONSE.UTILS;
using APPLICATION.DOMAIN.ENTITY.CHAT;
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
    /// Criar usuários.
    /// </summary>
    /// <param name="chatEntity"></param>
    /// <returns></returns>
    public async Task<ObjectResult> CreateChatAsync(ChatEntity chatEntity)
    {
        Log.Information($"[LOG INFORMATION] - SET TITLE {nameof(ChatSertvice)} - METHOD {nameof(CreateChatAsync)}\n");

        try
        {
            return await _chatRepository.CreateAsync(chatEntity)
                .ContinueWith(async astaskResult =>
                 {
                     await _unitOfWork.CommitAsync();

                    return new OkObjectResult(
                        new ApiResponse<ChatEntity>(
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
    /// Método responsável por retornar os chats de um usuário.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
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
                        .AsQueryable().Include(x => x.Messages).ThenInclude(m => m.User).ToList();

                    return new OkObjectResult(
                        new ApiResponse<ChatEntity>(
                            true, HttpStatusCode.OK, chats, new List<DadosNotificacao>  {
                                new DadosNotificacao("Chats recuperados com sucesso!")
                            }));
                });
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
        }
    }
}
