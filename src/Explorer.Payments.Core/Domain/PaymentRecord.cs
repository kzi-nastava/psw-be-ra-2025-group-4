using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class PaymentRecord : Entity
    {
        public int TouristId { get; private set; }
        public int TourId { get; private set; }
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
            Price = price;
            PurchaseTime = DateTime.UtcNow;
        }
    }
}

