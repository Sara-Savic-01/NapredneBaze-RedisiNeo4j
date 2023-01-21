using Microsoft.AspNetCore.SignalR;

namespace Filmofil.Services.Hubs
{
    public class MessageHub : Hub
    {
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}
