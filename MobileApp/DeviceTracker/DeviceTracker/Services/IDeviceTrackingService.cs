using DeviceTracker.Models;
using System.Threading.Tasks;

namespace DeviceTracker.Services
{
    public interface IDeviceTrackingService
    {
        Task<Route[]> GetRoutes();
        Task<Route> GetRoute(string id);
        Task SaveRoute(Route route);
        Task DeleteRoute(string id);
    }
}
