using System;
using Watch.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Watch
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DependencyService.Register<CreateSqliteTable>();
            DependencyService.Register<SqliteService<InterviewerEntity>>();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
