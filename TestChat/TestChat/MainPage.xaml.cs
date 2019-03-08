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

            var stack = new StackLayout { Padding = new Thickness(5, 5, 5, 5) };   

            scroll.Content = stack;

            var stack1 = new StackLayout();
            stack1.Children.Add(scroll);
            stack1.Children.Add(entry);
            stack1.Children.Add(send);

            client.Message += async (sender, e) =>
            {
                var textlabel = new Label
                {
                    Text = e
                };

                stack.Children.Add(textlabel);
                await scroll.ScrollToAsync(0, entry.Y, true);
            };

            send.Clicked += async (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(entry.Text))
                    {
                        var textlabel = new Label
                        {
                            Text = entry.Text
                        };

                        stack.Children.Add(textlabel);
                        await scroll.ScrollToAsync(0, entry.Y, true);
                    }
                };

            Content = stack1;

            Task.Run(async () => await client.Init());
        }
    }
}
