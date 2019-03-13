using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace TestChat.Serverless
{
    public class ClientHandler
    {
        private readonly HubConnection _connection;

        public ClientHandler(string connectionString, string hubName, string userId)
        {
            var serviceUtils = new ServiceUtils(connectionString);

            var url = GetClientUrl(serviceUtils.Endpoint, hubName);

            _connection = new HubConnectionBuilder()
                .WithUrl(url, option =>
                {
                    option.AccessTokenProvider = () =>
                    {
                        return Task.FromResult(serviceUtils.GenerateAccessToken(url, userId));
                    };
                }).Build();

            _connection.On<string, string>("SendMessage",
                (string server, string message) =>
                {
                    var model = new MessageModel
                    {
                        Name =server,
                        Message = message
                    };
                    Message?.Invoke(this, model);                    
                });
        }

        public async Task StartAsync()
        {
            await _connection.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await _connection.DisposeAsync();
        }

        private string GetClientUrl(string endpoint, string hubName)
        {
            return $"{endpoint}/client/?hub={hubName}";
        }

        public event EventHandler<MessageModel> Message;
    }
}
