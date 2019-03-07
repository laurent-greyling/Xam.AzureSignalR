using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Xam.AzureSignalR.Helpers
{
    public class ServerHandler
    {
        private static readonly HttpClient Client = new HttpClient();

        private readonly string _serverName;

        private readonly ServiceUtils _serviceUtils;

        private string ConnectionString => "";
        private string HubName => "ClientHub";

        private readonly string _endpoint;

        private PayloadMessage _defaultPayloadMessage;

        public ServerHandler(double sliderValue, Color textColor, double newValue)
        {
            _serverName = GenerateServerName();
            _serviceUtils = new ServiceUtils(ConnectionString);
            _endpoint = _serviceUtils.Endpoint;

            _defaultPayloadMessage = new PayloadMessage
            {
                Target = "BroadcastMessage",
                SliderValue = sliderValue,
                TextColor = textColor,
                NewValue = newValue
            };
        }

        public async Task Start()
        {
            await SendRequest("user", HubName, "UserID");
        }

        public async Task SendRequest(string command, string hubName, string arg = null)
        {
            string url = null;
            switch (command)
            {
                case "user":
                    url = GetSendToUserUrl(hubName, arg);
                    break;
                case "users":
                    url = GetSendToUsersUrl(hubName, arg);
                    break;
                case "group":
                    url = GetSendToGroupUrl(hubName, arg);
                    break;
                case "groups":
                    url = GetSendToGroupsUrl(hubName, arg);
                    break;
                case "broadcast":
                    url = GetBroadcastUrl(hubName);
                    break;
                default:
                    Debug.WriteLine($"Can't recognize command {command}");
                    break;
            }

            if (!string.IsNullOrEmpty(url))
            {
                var request = BuildRequest(url);

                var response = await Client.SendAsync(request);
                if (response.StatusCode != HttpStatusCode.Accepted)
                {
                    Debug.WriteLine($"Sent error: {response.StatusCode}");
                }
            }
        }

        private Uri GetUrl(string baseUrl)
        {
            return new UriBuilder(baseUrl).Uri;
        }

        private string GetSendToUserUrl(string hubName, string userId)
        {
            return $"{GetBaseUrl(hubName)}/users/{userId}";
        }

        private string GetSendToUsersUrl(string hubName, string userList)
        {
            return $"{GetBaseUrl(hubName)}/users/{userList}";
        }

        private string GetSendToGroupUrl(string hubName, string group)
        {
            return $"{GetBaseUrl(hubName)}/group/{group}";
        }

        private string GetSendToGroupsUrl(string hubName, string groupList)
        {
            return $"{GetBaseUrl(hubName)}/groups/{groupList}";
        }

        private string GetBroadcastUrl(string hubName)
        {
            return $"{GetBaseUrl(hubName)}";
        }

        private string GetBaseUrl(string hubName)
        {
            return $"{_endpoint}/api/v1/hubs/{hubName.ToLower()}";
        }

        private string GenerateServerName()
        {
            return $"{Environment.MachineName}_{Guid.NewGuid():N}";
        }

        private HttpRequestMessage BuildRequest(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, GetUrl(url));

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _serviceUtils.GenerateAccessToken(url, _serverName));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(JsonConvert.SerializeObject(_defaultPayloadMessage), Encoding.UTF8, "application/json");

            return request;
        }
    }

    //public class PayloadMessage
    //{
    //    public string Target { get; set; }

    //    public object[] Arguments { get; set; }
    //}
    public class PayloadMessage
    {
        public string Target { get; set; }
        public string Command { get; set; }
        public double SliderValue { get; set; }
        public Color TextColor { get; set; }
        public double NewValue { get; set; }
    }
}
