using DeviceTracker.Messages;
using DeviceTracker.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DeviceTracker.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        private string _userData;

        public string UserData { get => _userData; set => SetProperty(ref _userData, value); }

        public Command RefreshCommand { get; }
        public ICommand OpenWebCommand { get; }

        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
            RefreshCommand = new Command(async () => await ExecuteRefreshCommand());
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }

        private async Task ExecuteRefreshCommand()
        {
            IsBusy = true;

            UserData = "Loading...";

            try
            {
                var deviceTrackingService = DependencyService.Get<IDeviceTrackingService>();
                var routes = await deviceTrackingService.GetRoutes();
                UserData = $"You have createad {routes.Length} route(s)";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                UserData = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}