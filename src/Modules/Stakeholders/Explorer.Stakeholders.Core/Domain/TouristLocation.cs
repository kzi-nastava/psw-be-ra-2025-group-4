using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain
{
    public class TouristLocation
    {
        public int Id { get; private set; }
        public long UserId { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        protected TouristLocation() { }

        public TouristLocation(long userId, double latitude, double longitude)
        {
            UserId = userId;
            Latitude = latitude;
            Longitude = longitude;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateLocation(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

