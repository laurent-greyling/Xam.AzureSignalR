using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Watch.Serverless;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Watch
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FinishPage : ContentPage
	{
        private Label lbl = new Label
        {
            Text = "Completed",
            FontSize = 30,
            FontFamily = "Arial"
        };

        private Entry entry = new Entry();

        private Button btn = new Button
        {
            Text = "Continue",
            FontSize = 20
        };

        public FinishPage (string interviewerId)
		{
			InitializeComponent ();

            Title = interviewerId;

            var stack = new StackLayout
            {
                Children = { lbl, entry, btn}
            };            

            btn.Clicked += async (sender, e) =>
            {
                await Navigation.PushAsync(new MainPage());
            };

            Content = stack;

            //redo on text changed
            entry.TextChanged += async (sender, e) =>
            {
                await SendMessage(interviewerId, e.NewTextValue);
            };

            //initiate
            Task.Run(async () =>
            {
                await SendMessage(interviewerId);
            });
        }

        private async Task SendMessage(string interviewerId, string entryText = "")
        {
            var contentModel = new List<ContentModel>
            {
                new ContentModel { Type = "Label", Text = lbl.Text },
                new ContentModel { Type = "Entry", Text = entryText }
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