using System.Threading.Tasks;

namespace DeviceTracker.Services
{
    public interface ILocationConsent
    {
        Task GetLocationConsent();
    }
}
