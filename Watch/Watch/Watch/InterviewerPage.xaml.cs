using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watch.Serverless;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Watch
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class InterviewerPage : ContentPage
	{
        private Label label = new Label
        {
            FontFamily = "Arial",
            FontSize = 20
        };

        private Button button = new Button
        {
            Text = "Next",
            FontSize = 20
        };

        private bool IsManagerListening;

        private ContentModel categoryContent;

        public InterviewerPage(bool isCont = false, string interviewerId = "")
        {
            InitializeComponent();
            IsManagerListening = false;
            
            interviewerId = string.IsNullOrEmpty(interviewerId) ? Guid.NewGuid().ToString() : interviewerId;
            Title = interviewerId;

            var client = new ClientHandler("Interviewer", "SendMessageManager");
            Task.Run(async () => await client.StartAsync());
            var displayOnce = true;
           

            var mainStack = new StackLayout { Padding = new Thickness(5, 5, 5, 5) };

            var contentModel = new List<ContentModel>();

            if (isCont)
            {                
                label.Text = "2. What beverages do you enjoy?";
                var brands = new[] { "Cola", "Fanta", "Pepsi", "7Up" };

                var categories2 = new CheckBox(brands);

                mainStack.Children.Add(label);
                mainStack.Children.Add(categories2);
                mainStack.Children.Add(button);

                categoryContent = new ContentModel { Type = "CheckBox",  Categories = brands };

                categories2.Clicked += async (sender, args) =>
                {
                    categoryContent.SelectedCategries = categories2.CategoryLabels.ToArray();

                    if (IsManagerListening)
                    {
                        await SendMessage(categoryContent, interviewerId);
                    }
                };

                button.Clicked += async (sender, e) =>
                {
                    await Navigation.PushAsync(new FinishPage(interviewerId));
                };

                Content = mainStack;

                client.Message += async (sender, e) =>
                {
                    if (e.Message == "start")
                    {
                        IsManagerListening = true;
                        if (displayOnce)
                        {
                            await SendMessage(categoryContent, interviewerId);
                            displayOnce = false;
                        }
                    }

                    if (e.Message == "stop")
                    {
                        IsManagerListening = false;
                    }
                };
            }

            if (!isCont)
            {                
                label.Text = "1. Do you drink cooldrink";
                var yesNo = new[] { "Yes", "No" };

                var categories1 = new RadioButton(yesNo);
                
                mainStack.Children.Add(label);
                mainStack.Children.Add(categories1);
                mainStack.Children.Add(button);

                categoryContent = new ContentModel { Type = "RadioButton", Categories = yesNo };

                categories1.Clicked +=  async (sender, args) =>
                {
                    categoryContent.Text = categories1.CheckedValue;
                    if (IsManagerListening)
                    {
                        await SendMessage(categoryContent, interviewerId);
                    }
                };

                button.Clicked += async (sender, e) =>
                {
                    await Navigation.PushAsync(new InterviewerPage(true, interviewerId));
                };

                Content = mainStack;

                client.Message += async (sender, e) =>
                {
                    if (e.Message == "start")
                    {
                        IsManagerListening = true;
                        if (displayOnce)
                        {
                            await SendMessage(categoryContent, interviewerId);
                            displayOnce = false;
                        }
                    }

                    if (e.Message == "stop")
                    {
                        IsManagerListening = false;
                    }
                };
            }

            

            //initial screen share send if manager starts up first
            Task.Run(async () =>
            {
                if (IsManagerListening)
                {
                    await SendMessage(categoryContent, interviewerId);
                }
            });
        }

        private async Task SendMessage(ContentModel content, string interviewerId)
        {
            var contentModel = new List<ContentModel>
            {
                new ContentModel { Type = "Label", Text = label.Text },
                content ?? new ContentModel()
            };

            var contentView = JsonConvert.SerializeObject(contentModel);
            var message = new MessageModel
            {
                Id = interviewerId,
                Message = contentView
            };

            var server = new ServerHandler(message, "SendMessageInterviewer");
            await server.Start($"send user Manager");
        }
    }
}