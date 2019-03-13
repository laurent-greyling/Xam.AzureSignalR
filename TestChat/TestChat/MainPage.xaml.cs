using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestChat
{
    public partial class MainPage : ContentPage
    {
        private ChatMessageViewModel vm => new ChatMessageViewModel();

        public MainPage()
        {
            InitializeComponent();            

            var client = new Client();            
            var scroll = new ScrollView();
            var entry = new Entry();
            var textLabel = new Label();
            var send = new Button
            {
                Text = "Send"
            };

            var stack = new StackLayout { Padding = new Thickness(5, 5, 5, 5) };
            stack.Children.Add(textLabel);

            scroll.Content = stack;

            var stack1 = new StackLayout();

            stack1.Children.Add(scroll);
            stack1.Children.Add(entry);
            stack1.Children.Add(send);

            ////if you want a more live type update
            //entry.TextChanged += async (sender, e) =>
            //{
            //    var message = new MessageModel
            //    {
            //        Name = "UWP",
            //        Message = e.NewTextValue
            //    };

            //    await client.Broadcast(message);
            //};

            send.Clicked += async (sender, e) =>
            {
                if (!string.IsNullOrEmpty(entry.Text))
                {
                    var message = new MessageModel
                    {
                        Name = "UWP",
                        Message = entry.Text
                    };

                    await client.Broadcast(message);
                }
            };

            scroll.ScrollToAsync(0, entry.Y, true);

            Content = stack1;
            
            client.Message += (sender, e) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    textLabel.Text = $"{e.Name} {e.Message}";
                });
            };

            Task.Run(async () => await client.Init());
        }
    }
}
