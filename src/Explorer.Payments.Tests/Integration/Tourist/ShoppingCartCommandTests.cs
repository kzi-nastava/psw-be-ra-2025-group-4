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
            var secondAdd = controller.AddToCart(-4);
            var ok = secondAdd.Result as OkObjectResult;

           
            ok.ShouldNotBeNull();
            var dto = ok.Value as ShoppingCartDto;
            dto.ShouldNotBeNull();

            dto.TouristId.ShouldBe(2);
            dto.Items.Count.ShouldBe(2);
            dto.Items.Any(i => i.TourId == -2).ShouldBeTrue();
            dto.Items.Any(i => i.TourId == -4).ShouldBeTrue();
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

            // In Tours test data: -1 is not published (draft), while -4 is published.
            const int unpublishedTourId = -1;

     
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

        [Fact]
        public void SetGiftRecipient_sets_recipient_for_item()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId: "-1");
            var paymentsDb = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            var stakeholdersDb = scope.ServiceProvider.GetRequiredService<Explorer.Stakeholders.Infrastructure.Database.StakeholdersContext>();

            CleanCart(paymentsDb, touristId: -1);
            EnsureTestUsersExist(stakeholdersDb);

            (controller.AddToCart(-2).Result as OkObjectResult).ShouldNotBeNull();

            var dto = new SetGiftRecipientDto
            {
                RecipientUsername = "turista1@example.com"
            };

            var result = controller.SetGiftRecipient(-2, dto);
            var okResult = result.Result as OkObjectResult;

            okResult.ShouldNotBeNull();
            var cartDto = okResult.Value as ShoppingCartDto;
            cartDto.ShouldNotBeNull();
            cartDto.Items.Count.ShouldBe(1);
            cartDto.Items.Single().RecipientUserId.ShouldBe(-2);

            var storedCart = paymentsDb.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.TouristId == -1);

            storedCart.ShouldNotBeNull();
            storedCart.Items.Single().RecipientUserId.ShouldBe(-2);
        }

        [Fact]
        public void SetGiftRecipient_throws_when_recipient_not_found()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId: "-1");
            var paymentsDb = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            var stakeholdersDb = scope.ServiceProvider.GetRequiredService<Explorer.Stakeholders.Infrastructure.Database.StakeholdersContext>();

            CleanCart(paymentsDb, touristId: -1);
            EnsureTestUsersExist(stakeholdersDb);

            (controller.AddToCart(-2).Result as OkObjectResult).ShouldNotBeNull();

            var dto = new SetGiftRecipientDto
            {
                RecipientUsername = "nonexistent.user@example.com"
            };

            Should.Throw<Explorer.BuildingBlocks.Core.Exceptions.EntityValidationException>(() =>
                controller.SetGiftRecipient(-2, dto))
                .Message.ShouldBe("Recipient user not found.");
        }

        [Fact]
        public void SetGiftRecipient_throws_when_gifting_to_self()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId: "-1");
            var paymentsDb = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            var stakeholdersDb = scope.ServiceProvider.GetRequiredService<Explorer.Stakeholders.Infrastructure.Database.StakeholdersContext>();

            CleanCart(paymentsDb, touristId: -1);
            EnsureTestUsersExist(stakeholdersDb);

            (controller.AddToCart(-2).Result as OkObjectResult).ShouldNotBeNull();

            var dto = new SetGiftRecipientDto
            {
                RecipientUsername = "organizer"
            };

            Should.Throw<Explorer.BuildingBlocks.Core.Exceptions.EntityValidationException>(() =>
                controller.SetGiftRecipient(-2, dto))
                .Message.ShouldBe("Recipient user not found.");
        }

        private static void EnsureTestUsersExist(Explorer.Stakeholders.Infrastructure.Database.StakeholdersContext stakeholdersDb)
        {
            if (!stakeholdersDb.Users.Any(u => u.Id == -1))
            {
                var organizer = new Explorer.Stakeholders.Core.Domain.User(
                    "organizer",
                    "password",
                    Explorer.Stakeholders.Core.Domain.UserRole.Tourist,
                    true);
                stakeholdersDb.Users.Add(organizer);
                stakeholdersDb.Entry(organizer).Property("Id").CurrentValue = -1L;
                stakeholdersDb.SaveChanges();

                var organizerPerson = new Explorer.Stakeholders.Core.Domain.Person(-1L, "Organizer", "Test", "organizer@example.com");
                stakeholdersDb.People.Add(organizerPerson);
            }

            if (!stakeholdersDb.Users.Any(u => u.Id == -2))
            {
                var participant = new Explorer.Stakeholders.Core.Domain.User(
                    "turista1@example.com",
                    "password",
                    Explorer.Stakeholders.Core.Domain.UserRole.Tourist,
                    true);
                stakeholdersDb.Users.Add(participant);
                stakeholdersDb.Entry(participant).Property("Id").CurrentValue = -2L;
                stakeholdersDb.SaveChanges();

                var participantPerson = new Explorer.Stakeholders.Core.Domain.Person(-2L, "Turista", "One", "turista1@example.com");
                stakeholdersDb.People.Add(participantPerson);
            }

            stakeholdersDb.SaveChanges();
        }

        [Fact]
        public void Clears_entire_cart_and_resets_total_price()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId: "2");
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            CleanCart(dbContext, touristId: 2);

            
            (controller.AddToCart(-2).Result as OkObjectResult).ShouldNotBeNull();
            (controller.AddToCart(-4).Result as OkObjectResult).ShouldNotBeNull();

            
            var clearResult = controller.ClearCart();
            var okResult = clearResult.Result as OkObjectResult;

            
            okResult.ShouldNotBeNull();
            var dto = okResult.Value as ShoppingCartDto;
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
        public void Clearing_empty_cart_returns_empty_cart_without_error()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, personId: "2");
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            CleanCart(dbContext, touristId: 2);

            var clearResult = controller.ClearCart();
            var okResult = clearResult.Result as OkObjectResult;

            okResult.ShouldNotBeNull();
            var dto = okResult.Value as ShoppingCartDto;
            dto.ShouldNotBeNull();

            dto.TouristId.ShouldBe(2);
            dto.Items.ShouldBeEmpty();
            dto.TotalPrice.ShouldBe(0m);
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
