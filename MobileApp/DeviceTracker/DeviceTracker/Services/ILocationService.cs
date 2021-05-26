using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;

namespace DeviceTracker.Services
{
    public interface ILocationService
    {
        Task<Location> GetCurrentLocation(GeolocationAccuracy accuracy, CancellationToken token);
        Task<string> GetAddress(Position position);
        Task Start(CancellationToken token);
    }
}
