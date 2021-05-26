using Xamarin.Essentials;

namespace DeviceTracker.Models
{
    public class Point
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public long Timestamp { get; set; }

        public static implicit operator Point(Location location)
        {
            return new Point { Latitude = location.Latitude, Longitude = location.Longitude, Timestamp = location.Timestamp.ToUnixTimeSeconds() };
        }
    }
}
