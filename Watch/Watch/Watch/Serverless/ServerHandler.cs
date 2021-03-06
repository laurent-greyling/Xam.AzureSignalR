﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Watch.Serverless
{
    public class ServerHandler
    {
        private static readonly HttpClient Client = new HttpClient();

        private readonly string _serverName;

        private readonly ServiceUtils _serviceUtils;

        private readonly string _endpoint;

        private readonly PayloadMessage _defaultPayloadMessage;

        private string ConnectionString => "<your SignalR connection string here>";
        private readonly string _hubName;

        public ServerHandler(MessageModel message, string target)
        {
            _serverName = GenerateServerName();
            _serviceUtils = new ServiceUtils(ConnectionString);
            _endpoint = _serviceUtils.Endpoint;
            _hubName = "clienthub";
            _defaultPayloadMessage = new PayloadMessage
            {
                Target = target,
                Arguments = new object[]
                {
                    _serverName,
                    message
                }
            };
        }

        public async Task Start(string cmd)
        {
            if (cmd == null)
            {
                return;
            }
            var args = Regex.Split(cmd, " ");

            if (args.Length == 1 && args[0].Equals("broadcast"))
            {
                await SendRequest(args[0], _hubName);
            }
            else if (args.Length == 3 && args[0].Equals("send"))
            {
                await SendRequest(args[1], _hubName, args[2]);
            }
            else
            {
                Debug.WriteLine($"Can't recognize command {cmd}");
            }
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
                    Console.WriteLine($"Can't recognize command {command}");
                    break;
            }

            if (!string.IsNullOrEmpty(url))
            {
                var request = BuildRequest(url);

                var response = await Client.SendAsync(request);
                if (response.StatusCode != HttpStatusCode.Accepted)
                {
                    Console.WriteLine($"Sent error: {response.StatusCode}");
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

        private void ShowHelp()
        {
            Console.WriteLine("*********Usage*********\n" +
                              "send user <User Id>\n" +
                              "send users <User Id List>\n" +
                              "send group <Group Id>\n" +
                              "send groups <Group List>\n" +
                              "broadcast\n" +
                              "***********************");
        }
    }
}
