using APPLICATION.DOMAIN.HUB;
using APPLICATION.DOMAIN.UTILS.GLOBAL;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace APPLICATION.INFRAESTRUTURE.SIGNALR;

/// <summary>
/// Hub de notificações.
/// </summary>
public class HubNotifications : Hub
{
    private readonly ConcurrentDictionary<string, string> 
        _hubConnections = GlobalData.HubConnections;

    /// <summary>
    /// Método responsavel por envio de notificações para usuário
    /// </summary>
    /// <param name="notification"></param>
    /// <returns></returns>
    public async Task SendNotificationAsync(string userid, Notification notification)
    {
        await Clients.Client(userid).SendAsync("ReceberMensagem", notification);

        if (GlobalData.HubConnections is not null 
            && _hubConnections.TryGetValue(userid.ToLower(), out var connectionId))
        {
           await Clients
                .Client(connectionId).SendAsync("ReceberMensagem", notification);
        }
    }

    /// <summary>
    /// Método responsável pela conexão no hub.
    /// </summary>
    /// <returns></returns>
    public override Task OnConnectedAsync()
    {
        string userId
            = Context.GetHttpContext().Request.Query["userId"];

        AddOrUpdateInHub(userId);

        return base.OnConnectedAsync();
    }

    /// <summary>
    /// Método responsável pela disconexão do Hub.
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public override Task OnDisconnectedAsync(Exception exception)
    {
        string userId
            = Context.GetHttpContext().Request.Query["userId"];

        GlobalData.HubConnections
            .TryRemove(new KeyValuePair<string, string>(userId, Context.ConnectionId));

        return base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Adicionar 
    /// </summary>
    /// <param name="userId"></param>
    private void AddOrUpdateInHub(string userId)
    {
        if (_hubConnections.TryGetValue(userId, out var connectionId)) {
            _hubConnections
                .TryUpdate(userId, Context.ConnectionId, connectionId);
        }
        else { 
            _hubConnections
                .TryAdd(userId, Context.ConnectionId);
        }
    }
}