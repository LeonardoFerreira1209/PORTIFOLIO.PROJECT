using APPLICATION.DOMAIN.CONTRACTS.SERVICES.USER;
using APPLICATION.DOMAIN.DTOS.RESPONSE.UTILS;
using APPLICATION.DOMAIN.ENTITY.CHAT;
using APPLICATION.DOMAIN.UTILS.EXTENSIONS;
using APPLICATION.INFRAESTRUTURE.SIGNALR;
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
[Route("api/chat")]
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

    //[HttpPost]
    //public async Task<IActionResult> SendGlobalNotification(HubNotificationDto notification, string id)
    //{
    //    if (GlobalData.HubConnections is not null)
    //    {
    //        var connectionIds = GlobalData.HubConnections.Where(x => x.Key.Equals(id.ToLower())).Select(x => x.Value);

    //        foreach (var connectionId in connectionIds)
    //        {
    //            await _hubContext.Clients.Client(connectionId).SendAsync("ReceberMensagem", notification);
    //        }
    //    }
    //    return Ok();
    //}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpPost("create/chat")]
    //[CustomAuthorize(Claims.Role, "Delete")]
    [SwaggerOperation(Summary = "Recuperar dados do chat", Description = "Método responsável por recuperar dados do chat")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetChatsByUserAsync(ChatEntity chatEntity)
    {
        using (LogContext.PushProperty("Controller", "ChatManagerController"))
        using (LogContext.PushProperty("Payload", JsonConvert.SerializeObject(chatEntity)))
        using (LogContext.PushProperty("Metodo", "GetChatsByUser"))
        {
            return await Tracker.Time(()
                => _chatService.CreateChatAsync(chatEntity), "Chat criado com sucesso!");
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
}
