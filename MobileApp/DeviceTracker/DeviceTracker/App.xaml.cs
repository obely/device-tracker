using DeviceTracker.Messages;
using DeviceTracker.Services;
using DeviceTracker.Views;
using Xamarin.Forms;

namespace DeviceTracker
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            MessagingCenter.Subscribe<IAuthenticationService, LoggedInEvent>(this, nameof(LoggedInEvent), async (sender, _) =>
            {
                await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
            });
            MessagingCenter.Subscribe<IAuthenticationService>(this, nameof(LoggedOutEvent), async sender =>
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            });
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
