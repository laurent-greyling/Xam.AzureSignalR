using Microsoft.AspNet.SignalR.Client;
using System;
using Xam.AzureSignalR.Helpers;
using Xamarin.Forms;

namespace Xam.AzureSignalR
{
    public partial class MainPage : ContentPage
    {
        private IHubProxy hub;
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

        private double sliderValue;
        private double newValue;

        public MainPage()
        {
            InitializeComponent();

            SignalRClient signalR = new SignalRClient();
            signalR.InitializeSignalR();

            hub = signalR.SignalRHub;

            slider.ValueChanged += (sender, args) =>
            {
                sliderValue = slider.Value;
                newValue = args.NewValue;
                rotationLabel.Rotation = sliderValue;
                rotationLabel.TextColor = Color.FromRgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)); ;
                displayLabel.Text = String.Format("The Slider value is {0}", newValue);
                SendMessage("SLIDER", slider.Value, rotationLabel.TextColor, newValue);
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

        private void SendMessage(string command, double sliderValue, Color textColor, double newValue)
        {
            try
            {
                hub?.Invoke("newUpdate",
                    new object[] { command, sliderValue, textColor, newValue });
            }
            catch { }
        }
    }
}
