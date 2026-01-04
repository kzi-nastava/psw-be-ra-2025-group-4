using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using System;
using System.Collections.Generic;

namespace Explorer.Payments.Core.Domain
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
                throw new EntityValidationException("Tour name is required.");

            if (price < 0)
                throw new EntityValidationException("Price cannot be negative.");

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
