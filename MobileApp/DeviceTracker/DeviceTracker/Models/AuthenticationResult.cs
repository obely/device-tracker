using System.Collections.Generic;
using System.Security.Claims;
using Newtonsoft.Json;

namespace DeviceTracker.Models
{
    public class AuthenticationResult
    {
        public string IdToken { get; }
        public string AccessToken { get; }
        public string RefreshToken { get; }
        public IEnumerable<Claim> UserClaims { get; }
        public bool IsError { get; }
        public string Error { get; }

        [JsonConstructor]
        public AuthenticationResult(string idToken, string accessToken, string refreshToken, IEnumerable<Claim> userClaims)
        {
            IdToken = idToken;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            UserClaims = userClaims;
        }

        public AuthenticationResult(string error)
        {
            IsError = true;
            Error = error;
        }
    }
}
