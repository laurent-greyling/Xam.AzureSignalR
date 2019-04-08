using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace Watch.Serverless
{
    class ClientHandler
    {
        private HubConnection Connection { get; set; }
        private string ConnectionString => "<your SignalR connection string here>";

        public ClientHandler(string userId, string target)
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

            Connection.On<string, MessageModel>(target,
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
