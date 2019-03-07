using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xam.AzureSignalR.Helpers
{
    public class SignalRClient
    {
        public event EventHandler ValueChanged;

        public HubConnection HubConnection;
        private string ConnectionString => "";
        private string HubName => "ClientHub";

        public async Task InitializeSignalR(string userId)
        {
            var serviceUtils = new ServiceUtils(ConnectionString);
            var url = GetClientUrl(serviceUtils.Endpoint, HubName);

            HubConnection = new HubConnectionBuilder().WithUrl(url, option =>
            {
                option.AccessTokenProvider = () =>
                 {
                     return Task.FromResult(serviceUtils.GenerateAccessToken(url, userId));
                 };
            }).Build();
            
            //var hubConnection = new HubConnection("http://localhost:55580/signalr");
            //SignalRHub = _connection.CreateHubProxy("chat");
            //HubConnection.On("BroadcastMessage",
            //    (string server, string message) =>
            //    {
            //        Debug.WriteLine($"[{DateTime.Now.ToString()}] Received message from server {server}: {message}");
            //    });
            await HubConnection.InvokeAsync("BroadcastMessage");
            HubConnection.On<string, double, Color, double>("BroadcastMessage",
                    (command, sliderValue, textColor, newValue) => ValueChanged?.Invoke(this, new ValueChangedEventArgs(command, sliderValue, textColor, newValue)));

        }

        public async Task StartAsync()
        {
            await HubConnection.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await HubConnection.DisposeAsync();
        }

        private string GetClientUrl(string endpoint, string hubName)
        {
            return $"{endpoint}/client/?hub={hubName}";
        }
    }
}
