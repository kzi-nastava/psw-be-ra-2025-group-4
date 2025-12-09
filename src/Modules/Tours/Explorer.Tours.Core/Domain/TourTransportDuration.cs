using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public enum TourTransportType
    {
        Foot,
        Bike,
        Car
    }

    public class TourTransportDuration : ValueObject
    {
        public double Duration { get; }
        public TourTransportType Transport { get; }

        private TourTransportDuration() { }

        [JsonConstructor]
        public TourTransportDuration(double duration, TourTransportType transport)
        {
            Duration = duration;
            Transport = transport;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Duration;
            yield return Transport;
        }
    }
}
