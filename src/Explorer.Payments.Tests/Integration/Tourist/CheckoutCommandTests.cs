using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.API.Controllers.Tourist;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class CheckoutCommandTests : BasePaymentsIntegrationTest
    {
        public CheckoutCommandTests(PaymentsTestFactory factory) : base(factory) { }

        private static void CleanState(PaymentsContext db, int touristId)
        {
            var cart = db.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.TouristId == touristId);

            if (cart != null)
            {
                db.ShoppingCarts.Remove(cart);
                db.SaveChanges();
            }

            var tokens = db.TourPurchaseTokens
                .Where(t => t.TouristId == touristId)
                .ToList();

            if (tokens.Any())
            {
                db.TourPurchaseTokens.RemoveRange(tokens);
                db.SaveChanges();
            }
        }

        private static CheckoutController CreateController(IServiceScope scope, string personId)
        {
            return new CheckoutController(scope.ServiceProvider.GetRequiredService<ICheckoutService>())
            {
                ControllerContext = BuildContext(personId)
            };
        }

        [Fact]
        public void Checkout_creates_tokens_and_clears_cart()
        {
            const string TestTourist = "10001";
            var touristId = int.Parse(TestTourist);

            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            CleanState(db, touristId);

            
            var cart = new ShoppingCart(touristId);
            cart.AddItem(1, "HappyPath Tour", 20m);
            db.ShoppingCarts.Add(cart);
            db.SaveChanges();

            
            var controller = CreateController(scope, TestTourist);
            var ok = controller.Checkout().Result as OkObjectResult;
            ok.ShouldNotBeNull("Checkout should return 200 OK.");

            
            var tokens = ok!.Value as List<TourPurchaseTokenDto>;
            tokens.ShouldNotBeNull();
            tokens!.Count.ShouldBe(1);

            
            var after = db.ShoppingCarts.Include(c => c.Items).First(c => c.TouristId == touristId);
            after.Items.ShouldBeEmpty();
            after.TotalPrice.ShouldBe(0m);
        }

        [Fact]
        public void Checkout_throws_when_cart_is_empty()
        {
            const string TestTourist = "10002";
            var touristId = int.Parse(TestTourist);

            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            CleanState(db, touristId);

            var controller = CreateController(scope, TestTourist);

            Should.Throw<InvalidOperationException>(() => controller.Checkout());
        }

        [Fact]
        public void Checkout_creates_tokens_for_all_items_and_resets_total()
        {
            const string TestTourist = "10003";
            var touristId = int.Parse(TestTourist);

            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            CleanState(db, touristId);

            
            var cart = new ShoppingCart(touristId);
            cart.AddItem(1, "T1", 10m);
            cart.AddItem(2, "T2", 30m);
            db.ShoppingCarts.Add(cart);
            db.SaveChanges();

            
            var controller = CreateController(scope, TestTourist);
            var ok = controller.Checkout().Result as OkObjectResult;
            ok.ShouldNotBeNull("Checkout should return 200 OK.");

         
            var tokens = ok!.Value as List<TourPurchaseTokenDto>;
            tokens.ShouldNotBeNull();
            tokens!.Count.ShouldBe(2);

            var after = db.ShoppingCarts.Include(c => c.Items).First(c => c.TouristId == touristId);
            after.Items.ShouldBeEmpty();
            after.TotalPrice.ShouldBe(0m);
        }

        [Fact]
        public void Checkout_skips_existing_token()
        {
            const string TestTourist = "10004";
            var touristId = int.Parse(TestTourist);

            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            CleanState(db, touristId);

            
            db.TourPurchaseTokens.Add(new TourPurchaseToken(touristId, 1));
            db.SaveChanges();

          
            var cart = new ShoppingCart(touristId);
            cart.AddItem(1, "Once", 20m);
            db.ShoppingCarts.Add(cart);
            db.SaveChanges();

            var before = db.TourPurchaseTokens.Count(x => x.TouristId == touristId && x.TourId == 1);

           
            var controller = CreateController(scope, TestTourist);
            var ok = controller.Checkout().Result as OkObjectResult;
            ok.ShouldNotBeNull("Checkout should return 200 OK.");

           
            var after = db.TourPurchaseTokens.Count(x => x.TouristId == touristId && x.TourId == 1);
            after.ShouldBe(before);
        }
    }
}