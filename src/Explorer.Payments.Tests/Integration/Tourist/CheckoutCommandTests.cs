using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.API.Controllers.Tourist.Payments;
using Explorer.API.Hubs;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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

        private static void CleanState(PaymentsContext db, ToursContext toursDb, int touristId)
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

            var wallet = db.Wallets.FirstOrDefault(w => w.TouristId == touristId);
            if (wallet != null)
            {
                db.Wallets.Remove(wallet);
                db.SaveChanges();
            }

            var paymentRecords = db.PaymentRecords
                .Where(p => p.TouristId == touristId)
                .ToList();

            if (paymentRecords.Any())
            {
                db.PaymentRecords.RemoveRange(paymentRecords);
                db.SaveChanges();
            }

            // Clean tours used in tests
            var testTours = toursDb.Tours.Where(t => t.Id == 1 || t.Id == 2).ToList();
            if (testTours.Any())
            {
                toursDb.Tours.RemoveRange(testTours);
                toursDb.SaveChanges();
            }
        }

        private static void EnsureTestToursExist(ToursContext toursDb)
        {
            if (!toursDb.Tours.Any(t => t.Id == 1))
            {
                var tour1 = new Tour(1, "Test Tour 1", "Description 1", TourDifficulty.Easy, 
                    new List<string> { "test" }, TourStatus.Published, -11, 
                    new List<TourPoint>(), new List<Equipment>(), 20m, 
                    new List<TourTransportDuration>(), DateTime.UtcNow, null, 0.0);
                toursDb.Tours.Add(tour1);
                toursDb.SaveChanges();
            }

            if (!toursDb.Tours.Any(t => t.Id == 2))
            {
                var tour2 = new Tour(2, "Test Tour 2", "Description 2", TourDifficulty.Medium, 
                    new List<string> { "test" }, TourStatus.Published, -11, 
                    new List<TourPoint>(), new List<Equipment>(), 30m, 
                    new List<TourTransportDuration>(), DateTime.UtcNow, null, 0.0);
                toursDb.Tours.Add(tour2);
                toursDb.SaveChanges();
            }
        }

        private static CheckoutController CreateController(IServiceScope scope, string personId)
        {
            return new CheckoutController(
                scope.ServiceProvider.GetRequiredService<ICheckoutService>(),
                scope.ServiceProvider.GetRequiredService<Explorer.Stakeholders.API.Public.INotificationService>(),
                scope.ServiceProvider.GetRequiredService<IHubContext<MessageHub>>())
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
            var toursDb = scope.ServiceProvider.GetRequiredService<ToursContext>();
            CleanState(db, toursDb, touristId);
            EnsureTestToursExist(toursDb);

            var wallet = new Wallet(touristId);
            wallet.AddBalance(100m);
            db.Wallets.Add(wallet);
            
            var cart = new ShoppingCart(touristId);
            cart.AddItem(1, "HappyPath Tour", 20m);
            db.ShoppingCarts.Add(cart);
            db.SaveChanges();

            var controller = CreateController(scope, TestTourist);
            var result = controller.Checkout().Result;
            var ok = result.Result as OkObjectResult;
            ok.ShouldNotBeNull("Checkout should return 200 OK.");

            
            var tokens = ok!.Value as List<TourPurchaseTokenDto>;
            tokens.ShouldNotBeNull();
            tokens!.Count.ShouldBe(1);

            
            var after = db.ShoppingCarts.Include(c => c.Items).First(c => c.TouristId == touristId);
            after.Items.ShouldBeEmpty();
            after.TotalPrice.ShouldBe(0m);
        }

        [Fact]
        public void Checkout_returns_bad_request_when_cart_is_empty()
        {
            const string TestTourist = "10002";
            var touristId = int.Parse(TestTourist);

            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            var toursDb = scope.ServiceProvider.GetRequiredService<ToursContext>();
            CleanState(db, toursDb, touristId);

            var controller = CreateController(scope, TestTourist);

            var result = controller.Checkout().Result;
            result.Result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void Checkout_creates_tokens_for_all_items_and_resets_total()
        {
            const string TestTourist = "10003";
            var touristId = int.Parse(TestTourist);

            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            var toursDb = scope.ServiceProvider.GetRequiredService<ToursContext>();
            CleanState(db, toursDb, touristId);
            EnsureTestToursExist(toursDb);

            var wallet = new Wallet(touristId);
            wallet.AddBalance(100m);
            db.Wallets.Add(wallet);
            
            var cart = new ShoppingCart(touristId);
            cart.AddItem(1, "T1", 10m);
            cart.AddItem(2, "T2", 30m);
            db.ShoppingCarts.Add(cart);
            db.SaveChanges();

            var controller = CreateController(scope, TestTourist);
            var result = controller.Checkout().Result;
            var ok = result.Result as OkObjectResult;
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
            var toursDb = scope.ServiceProvider.GetRequiredService<ToursContext>();
            CleanState(db, toursDb, touristId);
            EnsureTestToursExist(toursDb);

            var wallet = new Wallet(touristId);
            wallet.AddBalance(100m);
            db.Wallets.Add(wallet);
            
            db.TourPurchaseTokens.Add(new TourPurchaseToken(touristId, 1));
            db.SaveChanges();

            var cart = new ShoppingCart(touristId);
            cart.AddItem(1, "Once", 20m);
            db.ShoppingCarts.Add(cart);
            db.SaveChanges();

            var before = db.TourPurchaseTokens.Count(x => x.TouristId == touristId && x.TourId == 1);

            var controller = CreateController(scope, TestTourist);
            var result = controller.Checkout().Result;
            var ok = result.Result as OkObjectResult;
            ok.ShouldNotBeNull("Checkout should return 200 OK.");

           
            var after = db.TourPurchaseTokens.Count(x => x.TouristId == touristId && x.TourId == 1);
            after.ShouldBe(before);
        }

        [Fact]
        public void GetPurchaseTokens_returns_all_tokens_for_tourist()
        {
            const string TestTourist = "10005";
            var touristId = int.Parse(TestTourist);

            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            var toursDb = scope.ServiceProvider.GetRequiredService<ToursContext>();
            CleanState(db, toursDb, touristId);
            EnsureTestToursExist(toursDb);

            db.TourPurchaseTokens.Add(new TourPurchaseToken(touristId, 1));
            db.TourPurchaseTokens.Add(new TourPurchaseToken(touristId, 2));
            db.SaveChanges();

            var controller = CreateController(scope, TestTourist);
            var result = controller.GetPurchaseTokens();
            var ok = result.Result as OkObjectResult;
            ok.ShouldNotBeNull("GetPurchaseTokens should return 200 OK.");

            var tokens = ok!.Value as List<TourPurchaseTokenDto>;
            tokens.ShouldNotBeNull();
            tokens!.Count.ShouldBe(2);
            tokens.Any(t => t.TourId == 1).ShouldBeTrue();
            tokens.Any(t => t.TourId == 2).ShouldBeTrue();
        }

        [Fact]
        public void GetPurchaseTokens_returns_empty_list_when_no_tokens_exist()
        {
            const string TestTourist = "10006";
            var touristId = int.Parse(TestTourist);

            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            var toursDb = scope.ServiceProvider.GetRequiredService<ToursContext>();
            CleanState(db, toursDb, touristId);

            var controller = CreateController(scope, TestTourist);
            var result = controller.GetPurchaseTokens();
            var ok = result.Result as OkObjectResult;
            ok.ShouldNotBeNull("GetPurchaseTokens should return 200 OK.");

            var tokens = ok!.Value as List<TourPurchaseTokenDto>;
            tokens.ShouldNotBeNull();
            tokens!.Count.ShouldBe(0);
        }

        [Fact]
        public void GetPurchaseTokens_returns_only_tokens_for_current_tourist()
        {
            const string TestTourist = "10007";
            const string OtherTourist = "10008";
            var touristId = int.Parse(TestTourist);
            var otherTouristId = int.Parse(OtherTourist);

            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            var toursDb = scope.ServiceProvider.GetRequiredService<ToursContext>();
            CleanState(db, toursDb, touristId);
            CleanState(db, toursDb, otherTouristId);
            EnsureTestToursExist(toursDb);

            db.TourPurchaseTokens.Add(new TourPurchaseToken(touristId, 1));
            db.TourPurchaseTokens.Add(new TourPurchaseToken(otherTouristId, 2));
            db.SaveChanges();

            var controller = CreateController(scope, TestTourist);
            var result = controller.GetPurchaseTokens();
            var ok = result.Result as OkObjectResult;
            ok.ShouldNotBeNull("GetPurchaseTokens should return 200 OK.");

            var tokens = ok!.Value as List<TourPurchaseTokenDto>;
            tokens.ShouldNotBeNull();
            tokens!.Count.ShouldBe(1);
            tokens[0].TourId.ShouldBe(1);
        }
    }
}