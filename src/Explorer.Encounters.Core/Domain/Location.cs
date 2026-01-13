using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Explorer.Encounters.Core.Domain
{
    public class Location : ValueObject
    {
        public double Longitude { get; private set; }
        public double Latitude { get; private set; }

        private const double EarthRadiusMeters = 6_371_000;

        private Location()
        {

        }

        [JsonConstructor]
        public Location(double longitude, double latitude)
        {
            if (latitude < -90 || latitude > 90)
                throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90 degrees.");
            if (longitude < -180 || longitude > 180)
                throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180 degrees.");

            Longitude = longitude;
            Latitude = latitude;
        }

        public double DistanceToMeters(Location other)
        {
            var lat1 = DegreesToRadians(Latitude);
            var lon1 = DegreesToRadians(Longitude);
            var lat2 = DegreesToRadians(other.Latitude);
            var lon2 = DegreesToRadians(other.Longitude);

            var dLat = lat2 - lat1;
            var dLon = lon2 - lon1;

            var a = Math.Pow(Math.Sin(dLat / 2), 2) +
                    Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dLon / 2), 2);

            var c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            return EarthRadiusMeters * c;
        }

        private static double DegreesToRadians(double degrees) => degrees * (Math.PI / 180);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Longitude;
            yield return Latitude;
        }
    }
}
