using APPLICATION.APPLICATION.SIGNALR;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.CHAT;
using APPLICATION.DOMAIN.DTOS.CHAT;
using APPLICATION.DOMAIN.DTOS.RESPONSE.CHAT;
using APPLICATION.DOMAIN.DTOS.RESPONSE.UTILS;
using APPLICATION.DOMAIN.UTILS.EXTENSIONS;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
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
public class ChatManagerController : ControllerBase
{
    private readonly IHubContext<HubNotifications> _hubContext;
    private readonly IChatService _chatService;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="hubContext"></param>
    public ChatManagerController(
        IHubContext<HubNotifications> hubContext, IChatService chatService)
    {
        _hubContext = hubContext; 
        _chatService = chatService;
    }

    /// <summary>
    /// Cria um chat.
    /// </summary>
    /// <param name="chatRequest"></param>
    /// <returns></returns>
    [HttpPost("create/chat")]
    //[CustomAuthorize(Claims.Role, "Delete")]
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
            return await Tracker.Time(()
                => _chatService.CreateChatAsync(chatRequest), "Criar chat");
        }
    }

    /// <summary>
    /// Cria e envia mensagem.
    /// </summary>
    /// <param name="chatMessageRequest"></param>
    /// <returns></returns>
    [HttpPost("send/message")]
    //[CustomAuthorize(Claims.Role, "Delete")]
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
            return await Tracker.Time(()
                => _chatService.SendMessageAsync(chatMessageRequest), "Enviar mensagem");
        }
    }

    /// <summary>
    /// Recuperar dados do chat
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("get/chats/by/user/{userId}")]
    //[CustomAuthorize(Claims.Role, "Delete")]
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
            return await Tracker.Time(()
                => _chatService.GetChatsByUserAsync(userId), "Recuperar chats por usuário");
        }
    }

    /// <summary>
    /// Recuperar dados das mensagens do chat.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("get/messages/by/chat/{chatId}")]
    //[CustomAuthorize(Claims.Role, "Delete")]
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
            return await Tracker.Time(()
                => _chatService.GetMessagesByChatAsync(chatId), "Recuperar mensagens por chat");
        }
    }
}
