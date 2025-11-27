using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class TourPoint : Entity
    {
        public int Id { get; set; }
        public int TourId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public int Order { get; private set; }

        public Tour? Tour { get; private set; }

        public TourPoint(int tourId, string name, string description, double latitude, double longitude, int order)
        {
            TourId = tourId;
            Name = name;
            Description = description;
            Latitude = latitude;
            Longitude = longitude;
            Order = order;
            Validate();
        }

        public void Update(string name, string description, double latitude, double longitude, int order)
        {
            Name = name;
            Description = description;
            Latitude = latitude;
            Longitude = longitude;
            Order = order;
            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Name cannot be empty.");
        }
    }
}
