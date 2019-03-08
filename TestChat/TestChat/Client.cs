using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace TestChat
{
    public class Client
    {
        public async Task Init()
        {
            try
            {
                var hubConnection = new HubConnectionBuilder().WithUrl("https://10.0.2.2:44333/").Build();

                hubConnection.On<string>("BroadcastMessage", msg =>
                {
                    Message?.Invoke(this, msg);
                });

                await hubConnection.StartAsync();
            }
            catch (Exception e)
            {

                throw;
            }
            
        }

        public event EventHandler<string> Message;
    }
}
