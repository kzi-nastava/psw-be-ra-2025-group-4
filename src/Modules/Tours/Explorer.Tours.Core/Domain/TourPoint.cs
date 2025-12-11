using Explorer.BuildingBlocks.Core.Domain;
using System.Net.Sockets;

namespace Explorer.Tours.Core.Domain
{
    public class TourPoint : Entity
    {
        public int Id { get; set; }
        public long TourId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public int Order { get; private set; }
        public string? ImageFileName { get; private set; }
        public string? Secret { get; private set; }

        public Tour? Tour { get; private set; }

        public TourPoint(long tourId, string name, string description, double latitude, double longitude, int order, string? imageFileName, string? secret)
        {
            TourId = tourId;
            Name = name;
            Description = description;
            Latitude = latitude;
            Longitude = longitude;
            Order = order;
            ImageFileName = imageFileName;
            Secret = secret;
            Validate();
        }

        public void Update(string name, string description, double latitude, double longitude, int order, string? imageFileName, string? secret)
        {
            Name = name;
            Description = description;
            Latitude = latitude;
            Longitude = longitude;
            Order = order;
            ImageFileName = imageFileName;
            Secret = secret;
            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Name cannot be empty.");
        }
    }
}
