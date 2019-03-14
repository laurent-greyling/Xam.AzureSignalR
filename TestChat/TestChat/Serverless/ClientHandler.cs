using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace TestChat.Serverless
{
    public class ClientHandler
    {
        private HubConnection Connection { get; set; }
        private string ConnectionString => "";

        public ClientHandler(string userId)
        {
            var serviceUtils = new ServiceUtils(ConnectionString);

            var url = GetClientUrl(serviceUtils.Endpoint);

            Connection = new HubConnectionBuilder()
                .WithUrl(url, option =>
                {
                    option.AccessTokenProvider = () =>
                    {
                        return Task.FromResult(serviceUtils.GenerateAccessToken(url, userId));
                    };
                }).Build();

            Connection.On<string, MessageModel>("SendMessage",
                (server, message) =>
                {
                    Message?.Invoke(this, message);                    
                });
        }

        public async Task StartAsync()
        {
            await Connection.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await Connection.DisposeAsync();
        }

        private string GetClientUrl(string endpoint)
        {
            return $"{endpoint}/client/?hub=clienthub";
        }

        public event EventHandler<MessageModel> Message;
    }
}
