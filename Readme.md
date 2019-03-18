# Xamarin Forms and AzureSignalR

In this project we set out to get [Azure SignalR](https://azure.microsoft.com/en-us/services/signalr-service/) working with Xamarin Forms (specifically Android and UWP). 

Azure SignalR allows you to add real-time functionality to push updated content to clients without the need to poll the server, or submit new HTTP requests for updates. Azure SignalR simplifies this process.

Apps that are best suited to SignalR:

- Apps that require high frequency updates from the server. Examples are gaming, voting, auction, maps, and GPS apps.
- Dashboards and monitoring apps. Examples include company dashboards and instant sales updates.
- Collaborative apps. Whiteboard apps and team meeting software are examples of collaborative apps.
- Apps that require notifications. Social networks, email, chat, games, travel alerts, and many other apps use notifications.

## Step 1. Setup Azure SignalR
1. Go to the Azure Portal
2. Add a resource and search for `SignalR Service`

![image](https://user-images.githubusercontent.com/17876815/54517871-2aca4600-4963-11e9-8268-6705b5eac4de.png)

3. Create your resource by following the on screen instructions.
4. Once created go the resource as here we will highlight/copy a few things you might need later for the project/process
    
    - Under `Keys` you will find your primary and secondary keys. These are needed later to connect to the signalr service via your app
    - Under `Settings` you will see feature flag toggles `Default`, `Serverless` and `Classic`. For the start of this project we can leave it on classic where we will have a web app acting as our hub. But as this project also include a serverless example, please take note that when you play with serverless the feature toggle should be on serverless and signalr service restarted.

## Step 2. Web app to act as SignalR Hub

For this example we follow the same chat app example as per the aspnet [samples](https://github.com/aspnet/AzureSignalR-samples/tree/master/samples/ChatRoom). To setup your environment please follow the steps as per the [samples](https://github.com/aspnet/AzureSignalR-samples/tree/master/samples/ChatRoom) or copy the code from `Xam.WebHub` from this repo.

There are few minor code differences, but one major difference for us is that we do not need UI for the web app as we are more interested in the xamarin forms part communicating correctly.

For our example, the hub created will use a model called `MessageModel` which is passed through instead of `string name, string message`.

```C#
//Sample App from MS
public void BroadcastMessage(string name, string message)
{
    Clients.All.SendAsync("broadcastMessage", name, message);
}

//Our Sample
public void BroadcastMessage(MessageModel message)
{
    Clients.All.SendAsync("BroadcastMessage", message);
}

public class MessageModel
{
    public string Name { get; set; }
    public string Message { get; set; }
}
```

If all the steps were followed as per [samples](https://github.com/aspnet/AzureSignalR-samples/tree/master/samples/ChatRoom), you should also have a UI component and you can start up your chat app and the web version should work correctly. If it complains about your connectionstring make sure you added the secrets to the `Asp.net Core Secrets Manager` of the project by running:

```
dotnet user-secrets set Azure:SignalR:ConnectionString "<your connection string>"
```

If you run the above line and you get an error stating something in the line that there is no secrets.json or there is no secrets id, then you need to do the following:

- right click on project file
- click on `Manage User Secrets`
- a file will be added called secrets.json
- as soon as you add your secret it should automagically appear in this file

## Step 3. Setup Xamarin Forms
In this repo the app called `TestChat` is the project we will focus on.

1. Create a Xamarin Forms up that uses `UWP` and `Android` as platforms. 
2. Install `Microsoft.AspNetCore.SignalR.Client` package to the shared app
3. In the `UWP Package.appxmanifest` app make sure that you have `Internet` and `Private Networks (Client & Server)` enabled.
4. Create your client which will communicate with SignalR via your created hub.
 
```C#
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
// This need to be included when debugging localhost, localhost will not work without this snippet if tested on https, you can also switch off Enable SSL in the web app which will allow localhost testing on http.
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

//This is used to broad cast a message to all the listening clients
        public async Task Broadcast(MessageModel message)
        {
            await Connection.InvokeAsync("BroadcastMessage", message);
        }

//This will be used listen for messages and handle them accordingly when received
        public event EventHandler<MessageModel> Message;
    }

```

5. Implement the client. For our purpose we do all code in the `MainPage.xaml.cs`. You can see how this is done [here](https://github.com/laurent-greyling/Xam.AzureSignalR/blob/master/TestChat/TestChat/MainPage.xaml.cs)

    - new up a client `var client = new Client();`
    - when send button is clicked broadcast the message = `await client.Broadcast(message);`
        - this will send the message to anyone that is also listening. It could be that you have one client listening and another broadcasting. For this app we are doing both.
    - add the handler we created to receive a message and display message
        ```c#
        client.Message += (sender, e) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    textLabel.Text = $"{e.Name} {e.Message}";
                });
            };
        ```
    - Initialise the client `Task.Run(async () => await client.Init());`

If all is done and said, start up an `Android` and an `UWP` app and see the magic happen

## Step 4. Xamarin Forms Serverless

For the serverless example you need to go to your `settings` panal for your SignalR service in azure and set the feature flag to serverless.

![image](https://user-images.githubusercontent.com/17876815/54532903-45afb100-4989-11e9-85cd-b04b6b2c7f78.png)

Once you have setup the serverless parts there will be no need for a web app to host the `ClientHub`. This will be a pure Xamarin Forms app with an Azure SignalR service and no other service in between.

For the serverless example see the `TestChat` app and the [serverless folder](https://github.com/laurent-greyling/Xam.AzureSignalR/tree/master/TestChat/TestChat)

- `ClientHandler` is to listen for messages
- `ServerHandler` will use the SignalR API to send the message
- In your `MainPage.xaml.cs` 
    - Create a new client as per the steps above for the Hub example
    - Instead of having a client broadcast we now will have the API Send the message once send button is clicked

    ```C#
    var server = new ServerHandler(message);
    await server.Start("send user UWP");    
    ```

For the serverless example your connection string to SignalR will be now part of your app, whereas with the hub solution your webapp will contain the connection string but not your Xamarin Forms App.

If you copy your connections string into the `ClientHandler` and the `ServerHandler` and start up the app in `Android` and `UWP` things should start working for you.

![serverless](https://user-images.githubusercontent.com/17876815/54533546-bc997980-498a-11e9-87db-9fd73976fd09.gif)

