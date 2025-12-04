using Explorer.Tours.Core.Domain;
using Shouldly;

namespace Explorer.Tours.Tests.Unit
{
    public class ShoppingCartTests
    {
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
            cart.TotalPrice.ShouldBe(100m);
        }



        [Fact]
        public void AddItem_does_not_add_duplicate_tour()
        {
            
            var cart = new ShoppingCart(touristId: 1);

            
            cart.AddItem(10, "Tour 1", 100m);
            cart.AddItem(10, "Tour 1", 100m);   

            
            cart.Items.Count.ShouldBe(1);
            cart.TotalPrice.ShouldBe(100m);
        }

        [Fact]
        public void AddItem_for_multiple_tours_sums_total_price()
        {
            
            var cart = new ShoppingCart(touristId: 1);

            
            cart.AddItem(10, "Tour 1", 100m);
            cart.AddItem(11, "Tour 2", 50m);

            
            cart.Items.Count.ShouldBe(2);
            cart.TotalPrice.ShouldBe(150m);
        }
    }
}
