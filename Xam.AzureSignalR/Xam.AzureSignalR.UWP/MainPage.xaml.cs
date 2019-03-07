
using Microsoft.AspNetCore.Builder;
using Xam.AzureSignalR.Helpers;

namespace Xam.AzureSignalR.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new Xam.AzureSignalR.App());
        }
    }
}
