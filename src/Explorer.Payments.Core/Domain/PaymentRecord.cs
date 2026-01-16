using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class PaymentRecord : Entity
    {
        public int TouristId { get; private set; }
        public int? TourId { get; private set; }
        public int? BundleId { get; private set; }
        public decimal Price { get; private set; }
        public DateTime PurchaseTime { get; private set; }

        private PaymentRecord() { }

        public PaymentRecord(int touristId, int tourId, decimal price)
        {
            if (touristId == 0) throw new ArgumentException("Invalid tourist id.", nameof(touristId));
            if (tourId == 0) throw new ArgumentException("Invalid tour id.", nameof(tourId));
            if (price < 0) throw new ArgumentException("Price cannot be negative.", nameof(price));
            
            TouristId = touristId;
            TourId = tourId;
            BundleId = null;
            Price = price;
            PurchaseTime = DateTime.UtcNow;
        }

        public PaymentRecord(int touristId, int bundleId, decimal price, bool isBundle)
        {
            if (touristId == 0) throw new ArgumentException("Invalid tourist id.", nameof(touristId));
            if (bundleId == 0) throw new ArgumentException("Invalid bundle id.", nameof(bundleId));
            if (price < 0) throw new ArgumentException("Price cannot be negative.", nameof(price));
            
            TouristId = touristId;
            TourId = null;
            BundleId = bundleId;
            Price = price;
            PurchaseTime = DateTime.UtcNow;
        }
    }
}

