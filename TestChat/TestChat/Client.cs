using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


namespace TestChat
{
    public class Client
    {
        private HubConnection Connection { get; set; }

        public async Task Init()
        {
            try
            {
                Connection = new HubConnectionBuilder().WithUrl("https://localhost:44333/clienthub", options =>
                {
#if DEBUG
                    options.HttpMessageHandlerFactory = (handler) =>
                    {
                        if (handler is HttpClientHandler clientHandler)
                        {
                            clientHandler.ServerCertificateCustomValidationCallback = ValidateCertificate;
                        }
                        return handler;
                    };
#endif
                }).Build();

                await Connection.StartAsync();

                Connection.On<MessageModel>("BroadcastMessage", message=> 
                {
                    Message?.Invoke(this, message);
                });
            }
            catch (Exception e)
            {

                throw;
            }
            
        }

        bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // TODO: You can do custom validation here, or just return true to always accept the certificate.
            // DO NOT use custom validation logic in a production application as it is insecure.
            return true;
        }

        public async Task Broadcast(MessageModel message)
        {
            await Connection.InvokeAsync("BroadcastMessage", message);
        }

        public event EventHandler<MessageModel> Message;
    }
}
