using System.Linq;
using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class ShoppingCartCommandTests : BaseToursIntegrationTest
    {
        public ShoppingCartCommandTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Adds_published_tour_to_cart()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId: "2");
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            
            var actionResult = controller.AddToCart(-2);
            var okResult = actionResult.Result as OkObjectResult;

            
            okResult.ShouldNotBeNull();
            var dto = okResult.Value as ShoppingCartDto;
            dto.ShouldNotBeNull();

            dto.TouristId.ShouldBe(2);
            dto.Items.Count.ShouldBe(1);
            var item = dto.Items.Single();
            item.TourId.ShouldBe(-2);
            item.TourName.ShouldBe("Test Tura 2");
            item.Price.ShouldBe(20.00m);
            dto.TotalPrice.ShouldBe(20.00m);

            
            var storedCart = dbContext.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.TouristId == 2);

            storedCart.ShouldNotBeNull();
            storedCart.Items.Count.ShouldBe(1);
            storedCart.Items.Single().TourId.ShouldBe(-2);
            storedCart.TotalPrice.ShouldBe(20.00m);
        }

        private static ShoppingCartController CreateController(IServiceScope scope, string personId)
        {
            return new ShoppingCartController(
                scope.ServiceProvider.GetRequiredService<IShoppingCartService>())
            {
                ControllerContext = BuildContext(personId)
            };
        }

        [Fact]
        public void GetCart_returns_existing_cart_for_tourist()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId: "2");

            
            controller.AddToCart(-2);

            
            var actionResult = controller.Get();
            var okResult = actionResult.Result as OkObjectResult;

            okResult.ShouldNotBeNull();
            var dto = okResult.Value as ShoppingCartDto;
            dto.ShouldNotBeNull();

            
            dto.TouristId.ShouldBe(2);
            dto.Items.Count.ShouldBe(1);
            var item = dto.Items.Single();
            item.TourId.ShouldBe(-2);
            item.Price.ShouldBe(20.00m);
            dto.TotalPrice.ShouldBe(20.00m);
        }

        [Fact]
        public void Removes_item_from_cart()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId: "2");   
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();


            var addResult = controller.AddToCart(-2);
            var addOk = addResult.Result as OkObjectResult;
            addOk.ShouldNotBeNull();

            
            var removeResult = controller.RemoveFromCart(-2);
            var removeOk = removeResult.Result as OkObjectResult;
            removeOk.ShouldNotBeNull();

            var dto = removeOk.Value as ShoppingCartDto;
            dto.ShouldNotBeNull();

           
            dto.TouristId.ShouldBe(2);
            dto.Items.Count.ShouldBe(0);
            dto.TotalPrice.ShouldBe(0m);

            
            var storedCart = dbContext.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.TouristId == 2);

            storedCart.ShouldNotBeNull();
            storedCart.Items.Count.ShouldBe(0);
            storedCart.TotalPrice.ShouldBe(0m);
        }

    }
}
