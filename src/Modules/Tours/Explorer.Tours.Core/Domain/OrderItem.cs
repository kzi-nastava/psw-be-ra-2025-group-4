using System;
using System.Collections.Generic;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class OrderItem : ValueObject
    {
        public int TourId { get; private set; }
        public string TourName { get; private set; }
        public decimal Price { get; private set; }

        private OrderItem() { }

        public OrderItem(int tourId, string tourName, decimal price)
        {
            if (string.IsNullOrWhiteSpace(tourName))
                throw new ArgumentException("Tour name is required.", nameof(tourName));

            if (price < 0)
                throw new ArgumentException("Price cannot be negative.", nameof(price));

            TourId = tourId;
            TourName = tourName.Trim();
            Price = price;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TourId;
            yield return TourName;
            yield return Price;
        }
    }
}
