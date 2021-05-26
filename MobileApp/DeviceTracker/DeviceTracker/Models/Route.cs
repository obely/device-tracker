using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xamarin.Forms.Maps;

namespace DeviceTracker.Models
{
    public class Route
    {
        public long Created { get; set; }
        public long Started { get; set; }
        public long Finished { get; set; }
        public Position StartLocation { get; set; }
        public Position FinishLocation { get; set; }

        [JsonIgnore]
        public DateTime StartedDate => DateTimeOffset.FromUnixTimeSeconds(Started).ToLocalTime().DateTime;
        [JsonIgnore]
        public DateTime FinishedDate => DateTimeOffset.FromUnixTimeSeconds(Finished).ToLocalTime().DateTime;
        [JsonIgnore]
        public TimeSpan Duration => TimeSpan.FromSeconds(Finished - Started);
        public List<Point> Points { get; set; }

        public Route()
        {
            Points = new List<Point>();
        }
    }
}
