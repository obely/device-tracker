using DeviceTracker.Messages;
using DeviceTracker.Services;
using Xamarin.Forms;

namespace DeviceTracker.ViewModels
{
    public class FlyoutHeaderViewModel : BaseViewModel
    {
        private string _userName;
        private string _userPicture;

        public string UserName { get => _userName; set => SetProperty(ref _userName, value); }
        public string UserPicture { get => _userPicture; set => SetProperty(ref _userPicture, value); }

        public FlyoutHeaderViewModel()
        {
            MessagingCenter.Subscribe<IAuthenticationService, LoggedInEvent>(this, nameof(LoggedInEvent), (sender, args) =>
           {
               UserName = args.Name;
               UserPicture = args.Picture;
           });
        }
    }
}
