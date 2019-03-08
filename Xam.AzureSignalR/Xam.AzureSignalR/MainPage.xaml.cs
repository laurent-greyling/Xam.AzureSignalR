using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using Xam.AzureSignalR.Helpers;
using Xamarin.Forms;
using ValueChangedEventArgs = Xamarin.Forms.ValueChangedEventArgs;

namespace Xam.AzureSignalR
{
    public partial class MainPage : ContentPage
    {
        private Random rnd = new Random();

        private Label rotationLabel = new Label
        {
            Text = "SIGNALR",
            FontSize = 60,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.CenterAndExpand
        };

        private Label displayLabel = new Label
        {
            Text = "(uninitialized)",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.CenterAndExpand
        };

        private Slider slider = new Slider
        {
            Maximum = 360
        };

        private Color TextColor;

        private SignalRClient signalR = new SignalRClient();
        private double sliderValue;
        private double newValue;

        public MainPage()
        {
            InitializeComponent();

            Task.Run(async ()=>
            {
                signalR.InitializeSignalR("UserID");

                await signalR.StartAsync();

                signalR.ValueChanged += (sender, e)=> 
                {
                    var vc = e as ValueChangedEventArgs;
                    sliderValue = slider.Value;
                    newValue = vc.NewValue;
                    rotationLabel.Rotation = sliderValue;
                    rotationLabel.TextColor = TextColor;
                    displayLabel.Text = String.Format("The Slider value is {0}", newValue);
                };
            });
            
            slider.ValueChanged += async (sender, args) =>
            {
                sliderValue = slider.Value;
                newValue = args.NewValue;
                TextColor = Color.FromRgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                rotationLabel.Rotation = sliderValue;
                rotationLabel.TextColor = TextColor;
                displayLabel.Text = String.Format("The Slider value is {0}", newValue);

                await SendMessage("SendMessage", sliderValue, TextColor, newValue);

                var server = new ServerHandler(slider.Value, rotationLabel.TextColor, newValue);
                await server.Start();

                //await SendMessage("user", slider.Value, rotationLabel.TextColor, newValue);
            };

            Title = "Basic Slider Code";
            Padding = new Thickness(10, 0);
            Content = new StackLayout
            {
                Children =
                {
                    rotationLabel,
                    slider,
                    displayLabel
                }
            };           
        }

        private async Task SendMessage(string command, double sliderValue, Color textColor, double newValue)
        {
            try
            {
                await signalR.HubConnection.SendAsync(command, new object[] { command, sliderValue, textColor, newValue });
                //hub?.Invoke("BroadcastMessage",
                //    new object[] { command, sliderValue, textColor, newValue });
            }
            catch { }
        }
    }
}
