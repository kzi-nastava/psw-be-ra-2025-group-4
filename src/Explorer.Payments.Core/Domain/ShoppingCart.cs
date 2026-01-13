using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Payments.Core.Domain
{

    public class ShoppingCart : AggregateRoot
    {
        private ShoppingCart() { }

        public int TouristId { get; private set; }

        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        public decimal TotalPrice { get; private set; }

        public ShoppingCart(int touristId)
        {
            if (touristId == 0)
                throw new EntityValidationException("Invalid tourist id.");

            TouristId = touristId;
            TotalPrice = 0;
        }

        public void AddItem(int tourId, string tourName, decimal price)
        {

            if (_items.Any(i => i.TourId == tourId)) return;

            _items.Add(new OrderItem(tourId, tourName, price));
            RecalculateTotal();
        }

        public void RemoveItem(int tourId)
        {
            var item = _items.FirstOrDefault(i => i.TourId == tourId);
            if (item == null)
            {
                throw new KeyNotFoundException($"Tour with ID {tourId} not found in cart.");
            }

            _items.Remove(item);
            RecalculateTotal();
        }


        public void Clear()
        {
            _items.Clear();
            RecalculateTotal();
        }

        private void RecalculateTotal()
        {
            TotalPrice = _items.Sum(i => i.Price);
        }

        public void AddItemWithPriceOverride(int tourId, string tourName, decimal finalPrice)
        {
            if (_items.Any(i => i.TourId == tourId)) return;

            _items.Add(new OrderItem(tourId, tourName, finalPrice));
            RecalculateTotal();
        }

        public void AddBundleItem(int bundleId, string bundleName, decimal price)
        {
            if (_items.Any(i => i.BundleId == bundleId)) return;

            _items.Add(new OrderItem(bundleId, bundleName, price, isBundle: true));
            RecalculateTotal();
        }

        public void RemoveBundleItem(int bundleId)
        {
            var item = _items.FirstOrDefault(i => i.BundleId == bundleId);
            if (item == null)
            {
                throw new KeyNotFoundException($"Bundle with ID {bundleId} not found in cart.");
            }

            _items.Remove(item);
            RecalculateTotal();
        }
    }
}
