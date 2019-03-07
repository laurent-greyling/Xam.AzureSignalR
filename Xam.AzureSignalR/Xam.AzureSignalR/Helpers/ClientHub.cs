using Microsoft.AspNetCore.SignalR;
using Xamarin.Forms;

namespace Xam.AzureSignalR.Helpers
{
    public class ClientHub : Hub
    {
        public void BroadcastMessage(string command, double sliderValue, Color textColor, double newValue)
        {
            Clients.All.SendAsync("BroadcastMessage", command, sliderValue, textColor, newValue);
        }

        public void Echo(string command, double sliderValue, Color textColor, double newValue)
        {
            Clients.Client(Context.ConnectionId).SendAsync("echo", command, sliderValue, textColor, newValue + " (echo from server)");
        }
    }
}
