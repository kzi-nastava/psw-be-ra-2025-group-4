using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Tours.Core.Domain
{
    public class MysteryTourOffer
    {
        public Guid Id { get; private set; }
        public int TouristId { get; private set; }
        public int TourId { get; private set; }
        public int DiscountPercent { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool Redeemed { get; private set; }

        public MysteryTourOffer(int touristId, int tourId, int discountPercent)
        {
            Id = Guid.NewGuid();
            TouristId = touristId;
            TourId = tourId;
            DiscountPercent = discountPercent;
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = CreatedAt.AddMinutes(10);
            Redeemed = false;
        }

        public void Redeem()
        {
            if (Redeemed) throw new InvalidOperationException("Offer already redeemed.");
            if (ExpiresAt < DateTime.UtcNow) throw new InvalidOperationException("Offer expired.");
            Redeemed = true;
        }
    }
}
