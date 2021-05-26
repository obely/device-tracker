using DeviceTracker.Models;
using DeviceTracker.Services;
using DeviceTracker.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DeviceTracker.ViewModels
{
    public class RoutesViewModel : BaseViewModel
    {
        private readonly IDeviceTrackingService _deviceTrackingService;
        private Route _selectedRoute;

        public ObservableCollection<Route> Routes { get; }
        public Command LoadRoutesCommand { get; }
        public Command<Route> RouteTapped { get; }

        public RoutesViewModel()
        {
            _deviceTrackingService = DependencyService.Get<IDeviceTrackingService>();

            Title = "Browse";
            Routes = new ObservableCollection<Route>();
            LoadRoutesCommand = new Command(async () => await ExecuteLoadRoutesCommand());

            RouteTapped = new Command<Route>(OnRouteSelected);
        }

        async Task ExecuteLoadRoutesCommand()
        {
            IsBusy = true;

            try
            {
                Routes.Clear();
                var routes = await _deviceTrackingService.GetRoutes();
                foreach (var route in routes)
                {
                    Routes.Add(route);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedRoute = null;
        }

        public Route SelectedRoute
        {
            get => _selectedRoute;
            set
            {
                SetProperty(ref _selectedRoute, value);
                OnRouteSelected(value);
            }
        }

        private async void OnRouteSelected(Route route)
        {
            if (route == null)
            {
                return;
            }

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(RouteDetailPage)}?{nameof(RouteDetailViewModel.RouteId)}={route.Started}");
        }
    }
}