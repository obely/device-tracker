using Xamarin.Essentials;

namespace DeviceTracker.Messages
{
    public class LocationReceivedEvent
    {
        public Location Location { get; }

        public LocationReceivedEvent(Location location)
        {
            Location = location;
        }
    }
}
