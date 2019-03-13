using Microsoft.AspNetCore.SignalR;

namespace Xam.WebHub
{
    public class ClientHub: Hub
    {
        
        public void BroadcastMessage(MessageModel message)
        {
            Clients.All.SendAsync("BroadcastMessage", message);
        }

        public void Echo(MessageModel message)
        {
            Clients.Client(Context.ConnectionId).SendAsync("echo", message.Name, message.Message + " (echo from server)");
        }
    }
}
