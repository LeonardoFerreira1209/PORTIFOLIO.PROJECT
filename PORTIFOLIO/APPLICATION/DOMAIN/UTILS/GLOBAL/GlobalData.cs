using APPLICATION.DOMAIN.DTOS.USER;
using System.Collections.Concurrent;

namespace APPLICATION.DOMAIN.UTILS.GLOBAL;

public static class GlobalData
{
    public static ConcurrentDictionary<string, string> HubConnection { get; set; } = new ConcurrentDictionary<string, string>();
    public static UserData GlobalUser { get; set; }
}
