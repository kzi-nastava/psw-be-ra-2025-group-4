using Microsoft.AspNetCore.SignalR;

namespace Explorer.API.Hubs
{
    public class HubContext
    {
        public static readonly Dictionary<long, HashSet<string>> ConnectedUsers
            = new Dictionary<long, HashSet<string>>();
    }
}
