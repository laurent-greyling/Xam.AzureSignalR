using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace TestChat
{
    public class Client
    {
        //public ChatMessageViewModel ChatVM { get; set; } = new ChatMessageViewModel();
        private HubConnection Connection { get; set; }

        public async Task Init()
        {
            try
            {
                Connection = new HubConnectionBuilder().WithUrl("https://127.0.0.1:44333/clienthub").Build();
                await Connection.StartAsync();

                Connection.On<string, string>("BroadcastMessage", (name, message)=> 
                {
                    var msg = new ChatMessage
                    {
                        Username = name,
                        Message = message
                    };

                    //ChatVM.Messages.Add(msg);
                    Message?.Invoke(this, message);
                });
            }
            catch (Exception e)
            {

                throw;
            }
            
        }

        public async Task Broadcast(string name, string message)
        {
            await Connection.InvokeAsync("BroadcastMessage", name, message);
        }

        public event EventHandler<string> Message;
    }
}
