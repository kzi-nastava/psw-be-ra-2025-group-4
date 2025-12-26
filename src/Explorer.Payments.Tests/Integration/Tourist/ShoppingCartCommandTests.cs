using System.Linq;
using Explorer.API.Controllers.Tourist;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class ShoppingCartCommandTests : BasePaymentsIntegrationTest
    {
        public ShoppingCartCommandTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Adds_published_tour_to_cart()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId: "2");
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // ACT
            var actionResult = controller.AddToCart(-2);
            var okResult = actionResult.Result as OkObjectResult;

            // ASSERT (controller vraća dto)
            okResult.ShouldNotBeNull();
            var dto = okResult.Value as ShoppingCartDto;
            dto.ShouldNotBeNull();

            dto.TouristId.ShouldBe(2);
            dto.Items.Count.ShouldBe(1);

            var item = dto.Items.Single();
            item.TourId.ShouldBe(-2);
            item.TourName.ShouldBe("Tour -2");   // <- novo (controller pravi)
            item.Price.ShouldBe(0m);             // <- novo (controller pravi)
            dto.TotalPrice.ShouldBe(0m);         // <- suma 0

            // ASSERT (upisano u Payments bazu)
            var storedCart = dbContext.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.TouristId == 2);

            storedCart.ShouldNotBeNull();
            storedCart.Items.Count.ShouldBe(1);
            storedCart.Items.Single().TourId.ShouldBe(-2);
            storedCart.TotalPrice.ShouldBe(0m);
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
            dto.Items.Single().TourId.ShouldBe(-2);
            dto.TotalPrice.ShouldBe(0m);
        }

        [Fact]
        public void Removes_item_from_cart()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId: "2");
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            (controller.AddToCart(-2).Result as OkObjectResult).ShouldNotBeNull();

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

        private static ShoppingCartController CreateController(IServiceScope scope, string personId)
        {
            return new ShoppingCartController(
                scope.ServiceProvider.GetRequiredService<IShoppingCartService>())
            {
                ControllerContext = BuildContext(personId)
            };
        }
    }
}
