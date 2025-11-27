using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public enum FacilityCategory
    {
        WC,
        Restaurant,
        Parking,
        Other
    }

    public class Facility : Entity
    {
        public string Name { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public FacilityCategory Category { get; private set; }

        public Facility(string name, double latitude, double longitude, FacilityCategory category)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Invalid name");

            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("Invalid latitude");

            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("Invalid longitude");

            Name = name;
            Latitude = latitude;
            Longitude = longitude;
            Category = category;
        }
    }
}
