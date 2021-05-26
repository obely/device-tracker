using DeviceTracker.Config;
using DeviceTracker.Models;
using DeviceTracker.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(DeviceTrackingService))]
namespace DeviceTracker.Services
{
    public class DeviceTrackingService : IDeviceTrackingService
    {
        private readonly HttpClient _httpClient;

        public DeviceTrackingService()
        {
            _httpClient = new HttpClient(new HttpClientAuthenticationHandler())
            {
                BaseAddress = new Uri(Configuration.RestApi)
            };
        }

        public async Task<Route> GetRoute(string id)
        {
            var response = await _httpClient.GetAsync($"routes/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var route = JsonConvert.DeserializeObject<Route>(json);

                return route;
            }

            return null;
        }

        public async Task<Route[]> GetRoutes()
        {
            var response = await _httpClient.GetAsync("routes");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var routes = JsonConvert.DeserializeObject<Route[]>(json);

                return routes;
            }

            return null;
        }

        public async Task SaveRoute(Route route)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var payload = JsonConvert.SerializeObject(route, serializerSettings);

            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("routes", content);
        }

        public async Task DeleteRoute(string id)
        {
            var response = await _httpClient.DeleteAsync($"routes/{id}");
        }

        #region Authentication

        private class HttpClientAuthenticationHandler : DelegatingHandler
        {
            private readonly IAuthenticationService _authenticationService;

            public HttpClientAuthenticationHandler() : this(new HttpClientHandler())
            {
            }
            public HttpClientAuthenticationHandler(HttpMessageHandler innerHandler) : base(innerHandler)
            {
                _authenticationService = DependencyService.Get<IAuthenticationService>();
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var authenticationResult = _authenticationService.AuthenticationResult;

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
                var response = await base.SendAsync(request, cancellationToken);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    authenticationResult = await _authenticationService.Refresh();
                    if (!authenticationResult.IsError)
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
                        return await base.SendAsync(request, cancellationToken);
                    }
                    else
                    {
                        await _authenticationService.Logout();
                    }
                }

                return response;
            }
        }

        #endregion
    }
}
