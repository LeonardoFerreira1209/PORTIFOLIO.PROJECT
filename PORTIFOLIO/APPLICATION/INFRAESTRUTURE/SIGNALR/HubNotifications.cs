using APPLICATION.DOMAIN.HUB;
using APPLICATION.DOMAIN.UTILS.GLOBAL;
using Microsoft.AspNetCore.SignalR;

namespace APPLICATION.INFRAESTRUTURE.SIGNALR;

/// <summary>
/// Hub de notificações.
/// </summary>
public class HubNotifications : Hub
{
    /// <summary>
    /// Método responsavel por envio de notificações para usuário.
    /// </summary>
    /// <param name="notification"></param>
    /// <returns></returns>
    public async Task SendNotification(Notification notification, string id)
    {
        await Clients.Client(id).SendAsync("ReceiveNotification", notification);
    }

    public override Task OnConnectedAsync()
    {
        var userId 
            = Context.GetHttpContext().Request.Query["userId"];

        GlobalData.HubConnection[userId] 
            = Context.ConnectionId;

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        var userId 
            = Context.GetHttpContext().Request.Query["userId"];

        GlobalData.HubConnection.TryRemove(userId, out _);

        return base.OnDisconnectedAsync(exception);
    }
}