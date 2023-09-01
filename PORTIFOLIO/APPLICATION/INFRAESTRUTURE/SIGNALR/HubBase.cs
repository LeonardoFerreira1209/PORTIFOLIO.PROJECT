using APPLICATION.DOMAIN.UTILS.GLOBAL;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace APPLICATION.INFRAESTRUTURE.SIGNALR;

/// <summary>
/// Classe base de Hubs.
/// </summary>
public abstract class HubBase : Hub
{
    public readonly ConcurrentDictionary<string, string>
        _hubConnections = new();

    /// <summary>
    /// ctor
    /// </summary>
    public HubBase(
        ConcurrentDictionary<string, string> hubConnections)
    {
        _hubConnections = hubConnections;
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

        GlobalData.HubNotifcationConnections
            .TryRemove(new KeyValuePair<string, string>(userId, Context.ConnectionId));

        return base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Adicionar 
    /// </summary>
    /// <param name="userId"></param>
    private void AddOrUpdateInHub(string userId)
    {
        if (_hubConnections.TryGetValue(userId, out var connectionId))
        {
            _hubConnections
                .TryUpdate(userId, Context.ConnectionId, connectionId);
        }
        else
        {
            _hubConnections
                .TryAdd(userId, Context.ConnectionId);
        }
    }
}
