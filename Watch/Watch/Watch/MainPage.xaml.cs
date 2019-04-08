using Xamarin.Forms;

namespace Watch
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            var managerButton = new Button
            {
                Text = "Manager",
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.White,
                BackgroundColor = Color.Blue
            };

            managerButton.Clicked += async (sender, e) =>
            {
                await Navigation.PushAsync(new ManagerPage());
            };

            var interviewerButton = new Button
            {
                Text = "Interviewer",
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.White,
                BackgroundColor = Color.Red
            };

            interviewerButton.Clicked += async (sender, e) =>
            {
                await Navigation.PushAsync(new InterviewerPage());
            };

            var stack = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Children = { managerButton, interviewerButton }
            };

            Content = stack;
        }
    }
}
