using DeviceTracker.Messages;
using DeviceTracker.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DeviceTracker.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private bool _isLoading;

        private IAuthenticationService _authenticationService;

        public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
        public Command LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);

            IsLoading = true;

            MessagingCenter.Subscribe<IAuthenticationService>(this, nameof(LoggedOutEvent), sender =>
            {
                IsLoading = false;
            });

            Task.Run(() => OnStartup());
        }

        public void OnStartup()
        {
            _authenticationService = DependencyService.Get<IAuthenticationService>();

            if (_authenticationService.AuthenticationResult == null)
            {
                IsLoading = false;
            }
        }

        private async void OnLoginClicked(object obj)
        {
            IsLoading = true;

            await _authenticationService.Login();
        }
    }
}
