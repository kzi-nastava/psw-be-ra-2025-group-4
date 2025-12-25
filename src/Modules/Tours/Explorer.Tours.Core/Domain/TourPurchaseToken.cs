
using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class TourPurchaseToken : Entity
    {
        public int TouristId { get; private set; }
        public int TourId { get; private set; }
        public DateTime PurchasedAt { get; private set; }

        private TourPurchaseToken() { }

        public TourPurchaseToken(int touristId, int tourId)
        {
            if (touristId == 0) throw new ArgumentException("Invalid tourist id.", nameof(touristId));
            if (tourId == 0) throw new ArgumentException("Invalid tour id.", nameof(tourId));
            TouristId = touristId;
            TourId = tourId;
            PurchasedAt = DateTime.UtcNow;
        }
    }
}
