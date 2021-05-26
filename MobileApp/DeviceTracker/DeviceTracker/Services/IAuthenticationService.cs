using System.Threading.Tasks;

using DeviceTracker.Models;

namespace DeviceTracker.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> Login();
        Task<AuthenticationResult> Refresh();
        Task<bool> Logout();
        AuthenticationResult AuthenticationResult { get; }
    }
}
