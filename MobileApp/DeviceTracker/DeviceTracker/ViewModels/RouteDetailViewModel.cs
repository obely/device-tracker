using DeviceTracker.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace DeviceTracker.ViewModels
{
    [QueryProperty(nameof(RouteId), nameof(RouteId))]
    public class RouteDetailViewModel : BaseViewModel
    {
        private readonly IDeviceTrackingService _deviceTrackingService;
        private readonly ILocationService _locationService;

        private string _routeId;
        private string _startAddress;
        private string _finishAddress;
        private DateTime _started;
        private DateTime _finished;
        private TimeSpan _duration;

        public event EventHandler<List<Models.Point>> RouteLoaded;

        public Command RemoveRouteCommand { get; }

        public RouteDetailViewModel()
        {
            _deviceTrackingService = DependencyService.Get<IDeviceTrackingService>();
            _locationService = DependencyService.Get<ILocationService>();

            RemoveRouteCommand = new Command(OnRemoveRoute);
        }

        public string StartAddress
        {
            get => _startAddress;
            set => SetProperty(ref _startAddress, value);
        }

        public string FinishAddress
        {
            get => _finishAddress;
            set => SetProperty(ref _finishAddress, value);
        }

        public DateTime Started
        {
            get => _started;
            set => SetProperty(ref _started, value);
        }

        public DateTime Finished
        {
            get => _finished;
            set => SetProperty(ref _finished, value);
        }

        public TimeSpan Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }

        public string RouteId
        {
            get
            {
                return _routeId;
            }
            set
            {
                _routeId = value;
                LoadItemId(value);
            }
        }

        private async void OnRemoveRoute(object obj)
        {
            await _deviceTrackingService.DeleteRoute(RouteId);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void LoadItemId(string routeId)
        {
            try
            {
                var route = await _deviceTrackingService.GetRoute(routeId);
                StartAddress = await _locationService.GetAddress(route.StartLocation);
                FinishAddress = await _locationService.GetAddress(route.FinishLocation);
                Started = route.StartedDate;
                Finished = route.FinishedDate;
                Duration = route.Duration;

                RouteLoaded?.Invoke(this, route.Points);
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
