using Auth0.OidcClient;
using DeviceTracker.Config;
using DeviceTracker.Droid.Services;
using DeviceTracker.Models;
using DeviceTracker.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(AuthenticationService))]
namespace DeviceTracker.Droid.Services
{
    public class AuthenticationService : AuthenticationServiceBase
    {
        private readonly Auth0Client _auth0Client;

        public AuthenticationService()
        {
            _auth0Client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = Configuration.Domain,
                ClientId = Configuration.ClientId
            });
        }

        protected override async Task<AuthenticationResult> DoLogin()
        {
            var auth0LoginResult = await _auth0Client.LoginAsync(new { audience = Configuration.Audience, scope = "openid profile email offline_access" });
            AuthenticationResult authenticationResult;

            if (!auth0LoginResult.IsError)
            {
                authenticationResult = new AuthenticationResult(auth0LoginResult.IdentityToken, auth0LoginResult.AccessToken, auth0LoginResult.RefreshToken, auth0LoginResult.User.Claims);
            }
            else
            {
                authenticationResult = new AuthenticationResult(auth0LoginResult.Error);
            }

            return authenticationResult;
        }

        protected override async Task<AuthenticationResult> DoRefresh()
        {
            var auth0RefreshTokenResult = await _auth0Client.RefreshTokenAsync(AuthenticationResult.RefreshToken);
            AuthenticationResult authenticationResult;

            if (!auth0RefreshTokenResult.IsError)
            {
                var userClaims = AuthenticationResult.UserClaims;
                authenticationResult = new AuthenticationResult(auth0RefreshTokenResult.IdentityToken, auth0RefreshTokenResult.AccessToken, auth0RefreshTokenResult.RefreshToken, userClaims);
            }
            else
            {
                authenticationResult = new AuthenticationResult(auth0RefreshTokenResult.Error);
            }

            return authenticationResult;
        }

        protected override async Task<bool> DoLogout()
        {
            var result = await _auth0Client.LogoutAsync();

            return result == IdentityModel.OidcClient.Browser.BrowserResultType.Success;
        }
    }
}