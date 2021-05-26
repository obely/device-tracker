using DeviceTracker.Messages;
using DeviceTracker.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

[assembly: Dependency(typeof(LocationService))]
namespace DeviceTracker.Services
{
    public class LocationService : ILocationService
    {
        public async Task<Location> GetCurrentLocation(GeolocationAccuracy accuracy, CancellationToken token)
        {
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var request = new GeolocationRequest(accuracy, TimeSpan.FromSeconds(30));
                var location = await Geolocation.GetLocationAsync(request, token);

                stopwatch.Stop();

                if (location != null)
                {
                    Debug.WriteLine($"Found current location in {stopwatch.Elapsed.TotalSeconds}s - Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");

                    return location;
                }
            }
            //catch (FeatureNotSupportedException fnsEx)
            //{
            //    // Handle not supported on device exception
            //}
            //catch (FeatureNotEnabledException fneEx)
            //{
            //    // Handle not enabled on device exception
            //}
            //catch (PermissionException pEx)
            //{
            //    // Handle permission exception
            //}
            catch (Exception exc)
            {
                Debug.WriteLine($"Failed to find current location: {exc}");
                throw;
            }

            throw new Exception("Failed to find current location");
        }

        public async Task<string> GetAddress(Position position)
        {
            try
            {
                var geocoder = new Geocoder();
                var addressList = await geocoder.GetAddressesForPositionAsync(position);
                var address = addressList.First();
                return address;
            }
            catch (Exception exc)
            {
                Debug.WriteLine($"Unable to get address for position {position}: {exc.Message}");
            }
            return $"{position.Latitude}, {position.Longitude}";
        }

        public async Task Start(CancellationToken token)
        {
            MessagingCenter.Send(new RouteStartedEvent(), nameof(RouteStartedEvent));

            while (true)
            {
                try
                {
                    var location = await GetCurrentLocation(GeolocationAccuracy.High, token);
                    MessagingCenter.Send(new LocationReceivedEvent(location), nameof(LocationReceivedEvent));

                    await Task.Delay(5000, token);
                }
                catch (OperationCanceledException)
                {
                    Debug.WriteLine($"{nameof(LocationService)}.{nameof(Start)} was cancelled");
                    break;
                }
                catch (Exception exc)
                {
                    Debug.WriteLine($"Unexpected exception occurred: {exc.Message}. Continue.");
                }
            }

            MessagingCenter.Send(new RouteStoppedEvent(), nameof(RouteStoppedEvent));
        }
    }
}
