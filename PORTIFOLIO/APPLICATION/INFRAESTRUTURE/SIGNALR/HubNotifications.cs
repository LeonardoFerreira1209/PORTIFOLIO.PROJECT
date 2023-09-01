using APPLICATION.DOMAIN.HUB;
using APPLICATION.DOMAIN.UTILS.GLOBAL;
using Microsoft.AspNetCore.SignalR;

namespace APPLICATION.INFRAESTRUTURE.SIGNALR;

/// <summary>
/// Hub de notificações.
/// </summary>
public class HubNotifications : HubBase
{
    public HubNotifications() 
        : base(GlobalData.HubNotifcationConnections) { }
    
    /// <summary>
    /// Método responsavel por envio de notificações para usuário
    /// </summary>
    /// <param name="notification"></param>
    /// <returns></returns>
    public async Task SendNotificationAsync(string userid, Notification notification)
    {
        await Clients.Client(userid).SendAsync("ReceberMensagem", notification);

        if (_hubConnections is not null
            && _hubConnections.TryGetValue(userid.ToLower(), out var connectionId))
        {
            await Clients
                 .Client(connectionId).SendAsync("ReceberMensagem", notification);
        }
    }
}