using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class CoinsBundleSale : Entity
    {
        public int CoinsBundleId { get; private set; }
        public decimal DiscountPercentage { get; private set; } // 10, 20, 30...
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public bool IsActive { get; private set; }

        private CoinsBundleSale() { }

        public CoinsBundleSale(int coinsBundleId, decimal discountPercentage,
            DateTime startDate, DateTime endDate)
        {
            if (coinsBundleId <= 0)
                throw new ArgumentException("Invalid bundle id.", nameof(coinsBundleId));
            if (discountPercentage <= 0 || discountPercentage > 100)
                throw new ArgumentException("Discount must be between 0 and 100.", nameof(discountPercentage));
            if (endDate <= startDate)
                throw new ArgumentException("End date must be after start date.");

            CoinsBundleId = coinsBundleId;
            DiscountPercentage = discountPercentage;
            StartDate = startDate;
            EndDate = endDate;
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public bool IsCurrentlyActive()
        {
            var now = DateTime.UtcNow;
            return IsActive && now >= StartDate && now <= EndDate;
        }

        public decimal CalculateDiscountedPrice(decimal originalPrice)
        {
            if (!IsCurrentlyActive()) return originalPrice;
            return originalPrice * (1 - DiscountPercentage / 100);
        }
    }
}