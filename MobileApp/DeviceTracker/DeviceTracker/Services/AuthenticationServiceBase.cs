using DeviceTracker.Converters.Json;
using DeviceTracker.Messages;
using DeviceTracker.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DeviceTracker.Services
{
    public abstract class AuthenticationServiceBase : IAuthenticationService
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public AuthenticationResult AuthenticationResult { get; private set; }

        public AuthenticationServiceBase()
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                Converters = { new ClaimConverter() }
            };

            var value = SecureStorage.GetAsync(nameof(AuthenticationResult)).Result;

            if (value != null)
            {
                try
                {
                    AuthenticationResult = JsonConvert.DeserializeObject<AuthenticationResult>(value, _jsonSerializerSettings);
                    OnLoggedIn();
                }
                catch (Exception exc)
                {
                    SecureStorage.Remove(nameof(AuthenticationResult));
                }
            }
        }

        public async Task<AuthenticationResult> Login()
        {
            AuthenticationResult = await DoLogin();

            if (!AuthenticationResult.IsError)
            {
                var value = JsonConvert.SerializeObject(AuthenticationResult, _jsonSerializerSettings);
                await SecureStorage.SetAsync(nameof(AuthenticationResult), value);

                OnLoggedIn();
            }

            return AuthenticationResult;
        }

        public async Task<bool> Logout()
        {
            var result = await DoLogout();

            if (result)
            {
                AuthenticationResult = null;
                SecureStorage.Remove(nameof(AuthenticationResult));

                OnLoggedOut();
            }

            return result;
        }

        public async Task<AuthenticationResult> Refresh()
        {
            AuthenticationResult = await DoRefresh();

            if (!AuthenticationResult.IsError)
            {
                var value = JsonConvert.SerializeObject(AuthenticationResult, _jsonSerializerSettings);
                await SecureStorage.SetAsync(nameof(AuthenticationResult), value);
            }

            return AuthenticationResult;
        }

        protected abstract Task<AuthenticationResult> DoLogin();
        protected abstract Task<bool> DoLogout();
        protected abstract Task<AuthenticationResult> DoRefresh();

        private void OnLoggedIn()
        {
            var claims = AuthenticationResult.UserClaims;

            var name = claims.First(c => c.Type == "name").Value;
            var email = claims.First(c => c.Type == "email").Value;
            var picture = claims.First(c => c.Type == "picture")?.Value;

            var loggedInEvent = new LoggedInEvent(name, email, picture);

            MessagingCenter.Send<IAuthenticationService, LoggedInEvent>(this, nameof(LoggedInEvent), loggedInEvent);
        }

        private void OnLoggedOut()
        {
            MessagingCenter.Send<IAuthenticationService>(this, nameof(LoggedOutEvent));
        }
    }
}
