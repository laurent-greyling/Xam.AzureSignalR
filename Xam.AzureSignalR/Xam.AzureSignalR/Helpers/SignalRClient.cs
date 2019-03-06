using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Http;
using System;
using System.Net;
using System.Net.Http;
using Xamarin.Forms;

namespace Xam.AzureSignalR.Helpers
{
    public class SignalRClient
    {
        private IHubProxy _hub;
        public event EventHandler ValueChanged;

        public IHubProxy SignalRHub { get { return _hub; } }

        public async void InitializeSignalR()
        {
            var hubConnection = new HubConnection("http://catirw.azurewebsites.net/");
            _hub = hubConnection.CreateHubProxy("clienthub");

            _hub.On<string, double, Color, double>("BroadcastMessage",
                    (command, sliderValue, textColor, newValue) => ValueChanged?.Invoke(this, new ValueChangedEventArgs(command, sliderValue, textColor, newValue)));
            
            await hubConnection.Start();
        }
    }
}
