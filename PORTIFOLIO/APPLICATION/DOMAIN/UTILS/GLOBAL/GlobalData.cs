using APPLICATION.DOMAIN.DTOS.USER;
using System.Collections.Concurrent;

namespace APPLICATION.DOMAIN.UTILS.GLOBAL;

public static class GlobalData
{
    public static ConcurrentDictionary<string, string> HubNotifcationConnections { get; set; } = new ConcurrentDictionary<string, string>();
    public static ConcurrentDictionary<string, string> HubChatConnections { get; set; } = new ConcurrentDictionary<string, string>();
    public static UserData GlobalUser { get; set; }
}
