using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Tours.Core.Domain
{
    public class Sale : AggregateRoot
    {
        public int AuthorId { get; private set; }
        public List<int> TourIds { get; private set; } = new List<int>();
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public int DiscountPercent { get; private set; }
        public bool IsActive { get; private set; }

        private Sale() { }

        public Sale(int authorId, List<int> tourIds, DateTime startDate, DateTime endDate, int discountPercent)
        {
            AuthorId = authorId;
            TourIds = tourIds ?? new List<int>();
            StartDate = startDate;
            EndDate = endDate;
            DiscountPercent = discountPercent;
            IsActive = true; // Automatski aktivna po kreiranju

            Validate();
        }

        private void Validate()
        {
            if (AuthorId == 0)
                throw new EntityValidationException("Invalid AuthorId.");

            if (TourIds == null || TourIds.Count == 0)
                throw new EntityValidationException("Sale must contain at least one tour.");

            if (StartDate >= EndDate)
                throw new EntityValidationException("End date must be after start date.");

            var maxDuration = TimeSpan.FromDays(14);
            if (EndDate - StartDate > maxDuration)
                throw new EntityValidationException("Sale duration cannot exceed 14 days.");

            if (DiscountPercent <= 0 || DiscountPercent > 100)
                throw new EntityValidationException("Discount percentage must be between 1 and 100.");
        }

        public void Update(List<int> tourIds, DateTime startDate, DateTime endDate, int discountPercent)
        {
            TourIds = tourIds ?? new List<int>();
            StartDate = startDate;
            EndDate = endDate;
            DiscountPercent = discountPercent;

            Validate();
        }

        public bool IsCurrentlyActive()
        {
            var now = DateTime.UtcNow;
            return IsActive && now >= StartDate && now <= EndDate;
        }

        public bool IsTourInSale(int tourId)
        {
            return TourIds.Contains(tourId);
        }

        public decimal CalculateDiscountedPrice(decimal originalPrice)
        {
            if (DiscountPercent <= 0 || DiscountPercent > 100)
                return originalPrice;

            var discountAmount = originalPrice * DiscountPercent / 100m;
            return Math.Round(originalPrice - discountAmount, 2);
        }
    }
}

