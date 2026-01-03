using System.Linq;
using Explorer.API.Controllers.Tourist.Payments;
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

            
            var actionResult = controller.AddToCart(-2);
            var okResult = actionResult.Result as OkObjectResult;

            
            okResult.ShouldNotBeNull();
            var dto = okResult.Value as ShoppingCartDto;
            dto.ShouldNotBeNull();

            dto.TouristId.ShouldBe(2);
            dto.Items.Count.ShouldBe(1);

            var item = dto.Items.Single();
            item.TourId.ShouldBe(-2);

            dto.TotalPrice.ShouldBe(dto.Items.Sum(i => i.Price));

            
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

           
            var actionResult = controller.Get();
            var okResult = actionResult.Result as OkObjectResult;

           
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

            
            var removeResult = controller.RemoveFromCart(-2);
            var removeOk = removeResult.Result as OkObjectResult;

            
            removeOk.ShouldNotBeNull();
            var dto = removeOk.Value as ShoppingCartDto;
            dto.ShouldNotBeNull();

            dto.TouristId.ShouldBe(2);
            dto.Items.ShouldBeEmpty();
            dto.TotalPrice.ShouldBe(0m);

         
            var storedCart = dbContext.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.TouristId == 2);

            storedCart.ShouldNotBeNull();
            storedCart.Items.ShouldBeEmpty();
            storedCart.TotalPrice.ShouldBe(0m);
        }

     

        [Fact]
        public void GetCart_when_cart_does_not_exist_returns_empty_or_not_found()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId: "2");
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            CleanCart(dbContext, touristId: 2);

           
            var actionResult = controller.Get();
            var result = actionResult.Result;

            if (result is OkObjectResult ok)
            {
                var dto = ok.Value as ShoppingCartDto;
                dto.ShouldNotBeNull();
                dto.TouristId.ShouldBe(2);
                dto.Items.ShouldBeEmpty();
                dto.TotalPrice.ShouldBe(0m);
            }
            else
            {
                (result is NotFoundResult || result is NotFoundObjectResult).ShouldBeTrue();
            }
        }

        [Fact]
        public void Adds_two_different_tours_to_cart_and_total_is_sum_of_item_prices()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId: "2");
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            CleanCart(dbContext, touristId: 2);

          
            (controller.AddToCart(-2).Result as OkObjectResult).ShouldNotBeNull();
            var secondAdd = controller.AddToCart(-3);
            var ok = secondAdd.Result as OkObjectResult;

           
            ok.ShouldNotBeNull();
            var dto = ok.Value as ShoppingCartDto;
            dto.ShouldNotBeNull();

            dto.TouristId.ShouldBe(2);
            dto.Items.Count.ShouldBe(2);
            dto.Items.Any(i => i.TourId == -2).ShouldBeTrue();
            dto.Items.Any(i => i.TourId == -3).ShouldBeTrue();
            dto.TotalPrice.ShouldBe(dto.Items.Sum(i => i.Price));

            
            var storedCart = dbContext.ShoppingCarts.Include(c => c.Items).FirstOrDefault(c => c.TouristId == 2);
            storedCart.ShouldNotBeNull();
            storedCart.Items.Count.ShouldBe(2);
            storedCart.TotalPrice.ShouldBe(storedCart.Items.Sum(i => i.Price));
        }

        [Fact]
        public void Adding_same_tour_twice_does_not_create_duplicate_item_for_same_tour()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId: "2");
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            CleanCart(dbContext, touristId: 2);

            (controller.AddToCart(-2).Result as OkObjectResult).ShouldNotBeNull();
            var secondAdd = controller.AddToCart(-2);
            var ok = secondAdd.Result as OkObjectResult;

           
            ok.ShouldNotBeNull();
            var dto = ok.Value as ShoppingCartDto;
            dto.ShouldNotBeNull();

            dto.Items.Count(i => i.TourId == -2).ShouldBe(1);
            dto.TotalPrice.ShouldBe(dto.Items.Sum(i => i.Price));

            var storedCart = dbContext.ShoppingCarts.Include(c => c.Items).FirstOrDefault(c => c.TouristId == 2);
            storedCart.ShouldNotBeNull();
            storedCart.Items.Count(i => i.TourId == -2).ShouldBe(1);
            storedCart.TotalPrice.ShouldBe(storedCart.Items.Sum(i => i.Price));
        }

        [Fact]
        public void Removing_non_existing_item_throws_and_cart_remains_unchanged()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId: "2");
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            CleanCart(dbContext, touristId: 2);
            (controller.AddToCart(-2).Result as OkObjectResult).ShouldNotBeNull();

           
            var ex = Should.Throw<KeyNotFoundException>(() => controller.RemoveFromCart(-9999));
            ex.Message.ShouldBe("Tour with ID -9999 not found in cart.");

         
            var storedCart = dbContext.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.TouristId == 2);

            storedCart.ShouldNotBeNull();
            storedCart.Items.Count.ShouldBe(1);
            storedCart.Items.Single().TourId.ShouldBe(-2);
            storedCart.TotalPrice.ShouldBe(storedCart.Items.Sum(i => i.Price));
        }


        [Fact]
        public void Add_unpublished_tour_throws_and_does_not_persist_item()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId: "2");
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            CleanCart(dbContext, touristId: 2);

            const int unpublishedTourId = -4;

     
            var ex = Should.Throw<Explorer.BuildingBlocks.Core.Exceptions.EntityValidationException>(() =>
                controller.AddToCart(unpublishedTourId)
            );

            ex.Message.ShouldBe("Only published tours can be added to cart.");

        
            var storedCart = dbContext.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.TouristId == 2);

            if (storedCart != null)
            {
                storedCart.Items.ShouldBeEmpty();
                storedCart.TotalPrice.ShouldBe(0m);
            }
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
