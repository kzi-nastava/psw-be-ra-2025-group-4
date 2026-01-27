using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class FavoriteTour : Entity
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public int TouristId { get; set; }
        public DateTime AddedAt { get; set; }

        public FavoriteTour(int tourId, int touristId, DateTime addedAt)
        {
            TourId = tourId;
            TouristId = touristId;
            AddedAt = addedAt;
            Validate();
        }

        private FavoriteTour()
        {
        }

        private void Validate()
        {
            if (TourId == 0)
                throw new ArgumentException("TourId cannot be zero");

            if (TouristId == 0)
                throw new ArgumentException("TouristId cannot be zero");
        }
    }
}
