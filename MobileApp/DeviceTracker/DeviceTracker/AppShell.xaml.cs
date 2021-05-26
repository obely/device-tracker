using System;
using DeviceTracker.Services;
using DeviceTracker.Views;
using Xamarin.Forms;

namespace DeviceTracker
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(RouteDetailPage), typeof(RouteDetailPage));
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            var authenticationService = DependencyService.Get<IAuthenticationService>();
            await authenticationService.Logout();
        }
    }
}
