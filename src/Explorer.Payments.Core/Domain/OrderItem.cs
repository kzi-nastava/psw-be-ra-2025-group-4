using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using System;
using System.Collections.Generic;

namespace Explorer.Payments.Core.Domain
{
    public class OrderItem : ValueObject
    {
        public int TourId { get; private set; }
        public int? BundleId { get; private set; }
        public string TourName { get; private set; }
        public decimal Price { get; private set; }
        public int? RecipientUserId { get; private set; }

        private OrderItem() { }

        public OrderItem(int tourId, string tourName, decimal price, int? recipientUserId = null)
        {
            if (string.IsNullOrWhiteSpace(tourName))
                throw new EntityValidationException("Tour name is required.");

            if (price < 0)
                throw new EntityValidationException("Price cannot be negative.");

            TourId = tourId;
            BundleId = null;
            TourName = tourName.Trim();
            Price = price;
            RecipientUserId = recipientUserId;
        }

        public OrderItem(int bundleId, string bundleName, decimal price, bool isBundle, int? recipientUserId = null)
        {
            if (string.IsNullOrWhiteSpace(bundleName))
                throw new EntityValidationException("Bundle name is required.");

            if (price < 0)
                throw new EntityValidationException("Price cannot be negative.");

            TourId = 0;
            BundleId = bundleId;
            TourName = bundleName.Trim();
            Price = price;
            RecipientUserId = recipientUserId;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TourId;
            yield return BundleId ?? 0;
            yield return TourName;
            yield return Price;
            yield return RecipientUserId ?? 0;
        }
    }
}
