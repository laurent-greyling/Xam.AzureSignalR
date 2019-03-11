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
        private ChatMessageViewModel vm = new ChatMessageViewModel();

        public MainPage()
        {
            InitializeComponent();

            var client = new Client();
            var scroll = new ScrollView();
            
            var entry = new Entry();
            var send = new Button
            {
                Text = "Send"
            };

            var textLabel = new Label();
            var stack = new StackLayout { Padding = new Thickness(5, 5, 5, 5) };   

            scroll.Content = stack;

            var stack1 = new StackLayout();
            stack1.Children.Add(scroll);
            stack1.Children.Add(entry);
            stack1.Children.Add(send);

            send.Clicked += async (sender, e) =>
            {
                if (!string.IsNullOrEmpty(entry.Text))
                {
                    await client.Broadcast("UWP", entry.Text);
                }
            };

            client.Message += (sender, e) =>
            {
                textLabel.Text = e;                
            };

            stack.Children.Add(textLabel);
            scroll.ScrollToAsync(0, entry.Y, true);

            Content = stack1;
            Task.Run(async () => await client.Init());
        }
    }
}
