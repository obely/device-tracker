
using DeviceTracker.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace DeviceTracker.Views
{
    public partial class RouteDetailPage : ContentPage
    {
        public RouteDetailPage()
        {
            InitializeComponent();
            var viewModel = new RouteDetailViewModel();
            BindingContext = viewModel;

            viewModel.RouteLoaded += ViewModel_RouteLoaded;
        }

        private void ViewModel_RouteLoaded(object sender, List<Models.Point> route)
        {
            var firstLocation = route.First();
            var lastLocation = route.Last();

            var region = MapSpan.FromCenterAndRadius(new Position(lastLocation.Latitude, lastLocation.Longitude), Distance.FromKilometers(0.2));
            Map.MoveToRegion(region);

            Map.Pins.Clear();
            var pin = new Pin { Label = "A", Type = PinType.Place, Position = new Position(firstLocation.Latitude, firstLocation.Longitude) };
            Map.Pins.Add(pin);


            pin = new Pin { Label = "B", Type = PinType.Place, Position = new Position(lastLocation.Latitude, lastLocation.Longitude) };
            Map.Pins.Add(pin);

            var polyline = new Polyline
            {
                StrokeColor = Color.Black,
                StrokeWidth = 5
            };
            Map.MapElements.Add(polyline);

            foreach (var location in route)
            {
                polyline.Geopath.Add(new Position(location.Latitude, location.Longitude));
            }
        }
    }
}