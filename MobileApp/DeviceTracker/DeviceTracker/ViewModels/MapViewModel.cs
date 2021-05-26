using DeviceTracker.Messages;
using DeviceTracker.Models;
using DeviceTracker.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Point = DeviceTracker.Models.Point;

namespace DeviceTracker.ViewModels
{
    public class MapViewModel : BaseViewModel
    {
        private readonly ILocationService _locationService;
        private readonly IDeviceTrackingService _deviceTrackingService;
        private Route _route;

        private bool _isRunning;

        public bool IsRunning { get => _isRunning; set => SetProperty(ref _isRunning, value); }

        public Command StartCommand { get; }
        public Command StopCommand { get; }

        public event EventHandler<Position> NewLocationAdded;
        public event EventHandler RouteStarted;
        public event EventHandler RouteStopped;

        public MapViewModel()
        {
            Title = "Map";
            _locationService = DependencyService.Get<ILocationService>();
            _deviceTrackingService = DependencyService.Get<IDeviceTrackingService>();
            _route = new Route();

            StartCommand = new Command(OnStartClicked);
            StopCommand = new Command(OnStopClicked);

            MessagingCenter.Subscribe<RouteStartedEvent>(this, nameof(RouteStartedEvent), sender =>
            {
                _route = new Route() { Created = DateTimeOffset.UtcNow.ToUnixTimeSeconds() };
                IsRunning = true;
                RouteStarted?.Invoke(this, EventArgs.Empty);
            });

            MessagingCenter.Subscribe<RouteStoppedEvent>(this, nameof(RouteStoppedEvent), sender =>
            {
                IsRunning = false;
                RouteStopped?.Invoke(this, EventArgs.Empty);

                _deviceTrackingService.SaveRoute(_route);
            });

            MessagingCenter.Subscribe<LocationReceivedEvent>(this, nameof(LocationReceivedEvent), (message) =>
            {
                _route.Points.Add(message.Location);
                NewLocationAdded?.Invoke(this, new Position(message.Location.Latitude, message.Location.Longitude));
            });

            var locationConsent = DependencyService.Get<ILocationConsent>();
            locationConsent.GetLocationConsent();
        }

        public async Task<List<Point>> GetRoute()
        {
            if (_route.Points.Count == 0)
            {
                var location = await _locationService.GetCurrentLocation(GeolocationAccuracy.Low, CancellationToken.None);
                return new List<Point> { location };
            }

            return _route.Points;
        }

        public void OnStartClicked()
        {
            MessagingCenter.Send(new StartRouteCommand(), nameof(StartRouteCommand));
        }

        public void OnStopClicked()
        {
            MessagingCenter.Send(new StopRouteCommand(), nameof(StopRouteCommand));
        }
    }
}
