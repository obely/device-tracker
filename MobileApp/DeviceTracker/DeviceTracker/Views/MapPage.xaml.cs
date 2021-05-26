using DeviceTracker.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace DeviceTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        private readonly MapViewModel _viewModel;
        private readonly Polyline _polyline;

        public MapPage()
        {
            InitializeComponent();

            _viewModel = (MapViewModel)BindingContext;
            _polyline = new Polyline
            {
                StrokeColor = Color.Black,
                StrokeWidth = 5
            };
            Map.MapElements.Add(_polyline);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _viewModel.NewLocationAdded += _viewModel_NewLocationAdded;
            _viewModel.RouteStarted += _viewModel_RouteStarted;
            _viewModel.RouteStopped += _viewModel_RouteStopped;

            RenderRoute();
        }

        private void _viewModel_RouteStarted(object sender, System.EventArgs e)
        {
            Map.Pins.Clear();
            _polyline.Geopath.Clear();
        }

        private void _viewModel_RouteStopped(object sender, System.EventArgs e)
        {
            if (_polyline.Geopath.Count > 1)
            {
                Map.Pins.Add(new Pin { Label = "B", Type = PinType.Place, Position = _polyline.Geopath.Last() });
            }
        }

        private void _viewModel_NewLocationAdded(object sender, Position e)
        {
            _polyline.Geopath.Add(e);

            if (Map.Pins.Count == 0)
            {
                Map.Pins.Add(new Pin { Label = "A", Type = PinType.Place, Position = _polyline.Geopath.First() });
            }

            var lastPosition = _polyline.Geopath.Last();
            var region = MapSpan.FromCenterAndRadius(lastPosition, Distance.FromKilometers(0.2));
            Map.MoveToRegion(region);
        }

        private async Task RenderRoute()
        {
            var route = await _viewModel.GetRoute();

            var firstLocation = route.First();
            var lastLocation = route.Last();

            var region = MapSpan.FromCenterAndRadius(new Position(lastLocation.Latitude, lastLocation.Longitude), Distance.FromKilometers(0.2));
            Map.MoveToRegion(region);

            Map.Pins.Clear();
            var pin = new Pin { Label = "A", Type = PinType.Place, Position = new Position(firstLocation.Latitude, firstLocation.Longitude) };
            Map.Pins.Add(pin);

            if (!_viewModel.IsRunning && lastLocation != firstLocation)
            {
                pin = new Pin { Label = "B", Type = PinType.Place, Position = new Position(lastLocation.Latitude, lastLocation.Longitude) };
                Map.Pins.Add(pin);
            }

            _polyline.Geopath.Clear();
            foreach (var location in route)
            {
                _polyline.Geopath.Add(new Position(location.Latitude, location.Longitude));
            }
        }
    }
}