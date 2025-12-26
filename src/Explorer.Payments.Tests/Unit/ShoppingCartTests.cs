using System.Linq;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Unit
{
    public class ShoppingCartTests
    {
        [Fact]
        public void Constructor_throws_for_invalid_tourist_id()
        {
            Should.Throw<EntityValidationException>(() => new ShoppingCart(touristId: 0))
                .Message.ShouldBe("Invalid tourist id.");
        }

        [Fact]
        public void AddItem_adds_new_item_and_updates_total()
        {
            var cart = new ShoppingCart(touristId: 1);

            cart.AddItem(tourId: 10, tourName: "Test tour", price: 100m);

            cart.Items.Count.ShouldBe(1);
            var item = cart.Items.Single();
            item.TourId.ShouldBe(10);
            item.TourName.ShouldBe("Test tour");
            item.Price.ShouldBe(100m);

            
            cart.TotalPrice.ShouldBe(cart.Items.Sum(i => i.Price));
        }

        [Fact]
        public void AddItem_does_not_add_duplicate_tour()
        {
            var cart = new ShoppingCart(touristId: 1);

            cart.AddItem(10, "Tour 1", 100m);
            cart.AddItem(10, "Tour 1", 100m);

            cart.Items.Count.ShouldBe(1);
            cart.Items.Single().TourId.ShouldBe(10);
            cart.TotalPrice.ShouldBe(cart.Items.Sum(i => i.Price));
        }

        [Fact]
        public void AddItem_for_multiple_tours_sums_total_price()
        {
            var cart = new ShoppingCart(touristId: 1);

            cart.AddItem(10, "Tour 1", 100m);
            cart.AddItem(11, "Tour 2", 50m);

            cart.Items.Count.ShouldBe(2);
            cart.Items.Select(i => i.TourId).ShouldBe(new[] { 10, 11 }, ignoreOrder: true);
            cart.TotalPrice.ShouldBe(cart.Items.Sum(i => i.Price));
        }

        [Fact]
        public void RemoveItem_removes_item_and_updates_total()
        {
            var cart = new ShoppingCart(touristId: 1);
            cart.AddItem(10, "Tour 1", 100m);
            cart.AddItem(11, "Tour 2", 50m);

            cart.RemoveItem(10);

            cart.Items.Count.ShouldBe(1);
            cart.Items.Single().TourId.ShouldBe(11);
            cart.TotalPrice.ShouldBe(cart.Items.Sum(i => i.Price));
        }

        [Fact]
        public void RemoveItem_throws_when_item_not_in_cart()
        {
            var cart = new ShoppingCart(touristId: 1);

            Should.Throw<KeyNotFoundException>(() => cart.RemoveItem(10))
                .Message.ShouldBe("Tour with ID 10 not found in cart.");
        }

        [Fact]
        public void Clear_removes_all_items_and_resets_total()
        {
            var cart = new ShoppingCart(touristId: 1);
            cart.AddItem(10, "Tour 1", 100m);
            cart.AddItem(11, "Tour 2", 50m);

            cart.Clear();

            cart.Items.ShouldBeEmpty();
            cart.TotalPrice.ShouldBe(0m);
        }
    }
}
