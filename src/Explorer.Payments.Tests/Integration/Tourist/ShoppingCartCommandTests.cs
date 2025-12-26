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
        public void Adds_tour_to_cart()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId: "2");
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            CleanCart(dbContext, touristId: 2);

            // ACT
            var actionResult = controller.AddToCart(-2);
            var okResult = actionResult.Result as OkObjectResult;

            // ASSERT (controller returns dto)
            okResult.ShouldNotBeNull();
            var dto = okResult.Value as ShoppingCartDto;
            dto.ShouldNotBeNull();

            dto.TouristId.ShouldBe(2);
            dto.Items.Count.ShouldBe(1);

            var item = dto.Items.Single();
            item.TourId.ShouldBe(-2);

            // Stable assertion: total must equal sum of item prices (works for 0, 20, etc.)
            dto.TotalPrice.ShouldBe(dto.Items.Sum(i => i.Price));

            // ASSERT (persisted in Payments DB)
            var storedCart = dbContext.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.TouristId == 2);

            storedCart.ShouldNotBeNull();
            storedCart.TouristId.ShouldBe(2);
            storedCart.Items.Count.ShouldBe(1);
            storedCart.Items.Single().TourId.ShouldBe(-2);
            storedCart.TotalPrice.ShouldBe(storedCart.Items.Sum(i => i.Price));
        }

        [Fact]
        public void GetCart_returns_existing_cart_for_tourist()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId: "2");
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            CleanCart(dbContext, touristId: 2);

            controller.AddToCart(-2);

            // ACT
            var actionResult = controller.Get();
            var okResult = actionResult.Result as OkObjectResult;

            // ASSERT
            okResult.ShouldNotBeNull();
            var dto = okResult.Value as ShoppingCartDto;
            dto.ShouldNotBeNull();

            dto.TouristId.ShouldBe(2);
            dto.Items.Count.ShouldBe(1);
            dto.Items.Single().TourId.ShouldBe(-2);
            dto.TotalPrice.ShouldBe(dto.Items.Sum(i => i.Price));
        }

        [Fact]
        public void Removes_item_from_cart()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId: "2");
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            CleanCart(dbContext, touristId: 2);

            (controller.AddToCart(-2).Result as OkObjectResult).ShouldNotBeNull();

            // ACT
            var removeResult = controller.RemoveFromCart(-2);
            var removeOk = removeResult.Result as OkObjectResult;

            // ASSERT (controller dto)
            removeOk.ShouldNotBeNull();
            var dto = removeOk.Value as ShoppingCartDto;
            dto.ShouldNotBeNull();

            dto.TouristId.ShouldBe(2);
            dto.Items.ShouldBeEmpty();
            dto.TotalPrice.ShouldBe(0m);

            // ASSERT (persisted)
            var storedCart = dbContext.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.TouristId == 2);

            storedCart.ShouldNotBeNull();
            storedCart.Items.ShouldBeEmpty();
            storedCart.TotalPrice.ShouldBe(0m);
        }

        private static void CleanCart(PaymentsContext db, int touristId)
        {
            var cart = db.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.TouristId == touristId);

            if (cart != null)
            {
                db.ShoppingCarts.Remove(cart);
                db.SaveChanges();
            }
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
