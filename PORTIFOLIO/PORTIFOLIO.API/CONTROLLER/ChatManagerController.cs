using APPLICATION.DOMAIN.CONTRACTS.FEATUREFLAGS;
using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.CHAT;
using APPLICATION.DOMAIN.DTOS.CHAT;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION.AUTH.CUSTOMAUTHORIZE.ATTRIBUTE;
using APPLICATION.DOMAIN.DTOS.RESPONSE.CHAT;
using APPLICATION.DOMAIN.DTOS.RESPONSE.UTILS;
using APPLICATION.DOMAIN.ENUMS;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PORTIFOLIO.API.CONTROLLER.BASE;
using Serilog.Context;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using StatusCodes = Microsoft.AspNetCore.Http.StatusCodes;

namespace PORTIFOLIO.API.CONTROLLER;

/// <summary>
/// ChatController
/// </summary>
[ApiController]
[Route("api/chatmanager")]
[EnableCors("CorsPolicy")]
public class ChatManagerController : BaseControllercs
{
    private readonly IChatService _chatService;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="hubContext"></param>
    public ChatManagerController(
        IChatService chatService, IFeatureFlags featureFlags, IUnitOfWork unitOfWork) : base(featureFlags, unitOfWork)
    {
        _chatService = chatService;
    }

    /// <summary>
    /// Endpoint responsável por criar um char entre dois usuários.
    /// </summary>
    /// <param name="chatRequest"></param>
    /// <returns></returns>
    [HttpPost("create/chat")]
    [CustomAuthorize(Claims.Chat, "Post")]
    [SwaggerOperation(Summary = "Recuperar dados do chat", Description = "Método responsável por recuperar dados do chat")]
    [ProducesResponseType(typeof(ApiResponse<ChatResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ChatResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ChatResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateChatAsync(ChatRequest chatRequest)
    {
        using (LogContext.PushProperty("Controller", "ChatManagerController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(chatRequest)))
        using (LogContext.PushProperty("Metodo", "CreateChatAsync"))
        {
            return await ExecuteAsync(nameof(CreateChatAsync),
                () => _chatService.CreateChatAsync(chatRequest), "Criar chat");
        }
    }

    /// <summary>
    /// Endpoint responsável por criar uma nova mensagem no chat e enviar para o remetente.
    /// </summary>
    /// <param name="chatMessageRequest"></param>
    /// <returns></returns>
    [HttpPost("send/message")]
    [CustomAuthorize(Claims.Chat, "Post")]
    [SwaggerOperation(Summary = "Recuperar dados do chat", Description = "Método responsável por recuperar dados do chat")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendMessageAsync(ChatMessageRequest chatMessageRequest)
    {
        using (LogContext.PushProperty("Controller", "ChatManagerController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(chatMessageRequest)))
        using (LogContext.PushProperty("Metodo", "SendMessageAsync"))
        {
            return await ExecuteAsync(nameof(SendMessageAsync),
               () => _chatService.SendMessageAsync(chatMessageRequest), "Enviar mensagem");
        }
    }

    /// <summary>
    /// Endpoint responsável por recuperar os dados de um chat através do Id usuário.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("get/chats/by/user/{userId}")]
    [CustomAuthorize(Claims.Chat, "Get")]
    [SwaggerOperation(Summary = "Recuperar dados do chat", Description = "Método responsável por recuperar dados do chat")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetChatsByUserAsync(Guid userId)
    {
        using (LogContext.PushProperty("Controller", "ChatManagerController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(userId)))
        using (LogContext.PushProperty("Metodo", "GetChatsByUser"))
        {
            return await ExecuteAsync(nameof(GetChatsByUserAsync),
               () => _chatService.GetChatsByUserAsync(userId), "Recuperar chats por usuário");
        }
    }

    /// <summary>
    /// Endpoint responsável por recuperar todas as mensagens de um chat, pelo Id do chat.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("get/messages/by/chat/{chatId}")]
    [CustomAuthorize(Claims.Chat, "Get")]
    [SwaggerOperation(Summary = "Recuperar mensagens do chat", Description = "Método responsável por recuperar mensagens do chat")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMessagesByChatAsync(Guid chatId)
    {
        using (LogContext.PushProperty("Controller", "ChatManagerController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(chatId)))
        using (LogContext.PushProperty("Metodo", "GetMessagesByChatAsync"))
        {
            return await ExecuteAsync(nameof(GetMessagesByChatAsync),
               () => _chatService.GetMessagesByChatAsync(chatId), "Recuperar mensagens por chat");
        }
    }
}
