using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;

namespace DeviceTracker.Config
{
    public static class Configuration
    {
        public static string Domain { get; }
        public static string ClientId { get; }
        public static string Audience { get; }
        public static string RestApi { get; }

        static Configuration()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "DeviceTracker.appsettings.json";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();

                var jObject = JObject.Parse(json);

                Domain = jObject["Authentication"]["Domain"].ToString();
                ClientId = jObject["Authentication"]["ClientId"].ToString();
                Audience = jObject["Authentication"]["Audience"].ToString();
                RestApi = jObject["Service"]["Url"].ToString();
            }
        }
    }
}
