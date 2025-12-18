using System.Collections.Generic;
using System.Linq;
using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Shopping;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class CheckoutCommandTests : BaseToursIntegrationTest
    {
        public CheckoutCommandTests(ToursTestFactory factory) : base(factory) { }

        
        private static void CleanState(ToursContext db, int touristId)
        {
            var cart = db.ShoppingCarts.Include(c => c.Items).FirstOrDefault(c => c.TouristId == touristId);
            if (cart != null) { db.Remove(cart); db.SaveChanges(); }

            var tokens = db.TourPurchaseTokens.Where(t => t.TouristId == touristId).ToList();
            if (tokens.Count > 0) { db.TourPurchaseTokens.RemoveRange(tokens); db.SaveChanges(); }
        }

        private static Tour MakeTour(ToursContext db, string name, decimal price, int authorId = 999)
        {
            var t = new Tour(name, "Desc", TourDifficulty.Easy, authorId, new List<TourTransportDuration>());
            t.SetStatus(TourStatus.Published);
            t.SetPrice(price);
            db.Tours.Add(t);
            db.SaveChanges();
            return t;
        }

        

        [Fact]
        public void AddToCart_then_Checkout_creates_tokens_and_clears_cart()
        {
            const string TestTourist = "10001";
            var touristId = int.Parse(TestTourist);

            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

           
            CleanState(db, touristId);
            var tour = MakeTour(db, "HappyPath Tour", 20m);

            
            var cartCtl = new ShoppingCartController(
                scope.ServiceProvider.GetRequiredService<IShoppingCartService>())
            { ControllerContext = BuildContext(TestTourist) };

            var addRes = cartCtl.AddToCart((int)tour.Id);
            var addOk = addRes.Result as OkObjectResult;
            addOk.ShouldNotBeNull("AddToCart mora vratiti 200 OK.");

            var cartDto = addOk.Value as ShoppingCartDto;
            cartDto.ShouldNotBeNull();
            cartDto!.TouristId.ShouldBe(touristId);
            cartDto.Items.Count.ShouldBe(1);
            cartDto.Items.Single().TourId.ShouldBe((int)tour.Id);
            cartDto.TotalPrice.ShouldBe(20m);

            // Checkout
            var checkoutCtl = new CheckoutController(
                scope.ServiceProvider.GetRequiredService<ICheckoutService>())
            { ControllerContext = BuildContext(TestTourist) };

            var chkRes = checkoutCtl.Checkout();
            var chkOk = chkRes.Result as OkObjectResult;
            chkOk.ShouldNotBeNull("Checkout mora vratiti 200 OK.");

            var tokens = chkOk.Value as List<TourPurchaseTokenDto>;
            tokens.ShouldNotBeNull();
            tokens!.Count.ShouldBeGreaterThan(0);

            
            var firstTokenId = tokens.First().Id;
            db.TourPurchaseTokens.AsNoTracking()
              .FirstOrDefault(t => t.Id == firstTokenId)
              .ShouldNotBeNull();

            
            var after = db.ShoppingCarts.Include(c => c.Items)
                                        .First(c => c.TouristId == touristId);
            after.Items.ShouldBeEmpty();
            after.TotalPrice.ShouldBe(0m);
        }

        [Fact]
        public void Checkout_throws_when_cart_is_empty()
        {
            const string TestTourist = "10002";
            var touristId = int.Parse(TestTourist);

            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
            CleanState(db, touristId);

            var checkoutCtl = new CheckoutController(
                scope.ServiceProvider.GetRequiredService<ICheckoutService>())
            { ControllerContext = BuildContext(TestTourist) };

            Should.Throw<InvalidOperationException>(() => checkoutCtl.Checkout());
        }

        [Fact]
        public void Checkout_creates_tokens_for_all_items_and_resets_total()
        {
            const string TestTourist = "10003";
            var touristId = int.Parse(TestTourist);

            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
            CleanState(db, touristId);

            var t1 = MakeTour(db, "T1", 10m);
            var t2 = MakeTour(db, "T2", 30m);

            var cartCtl = new ShoppingCartController(
                scope.ServiceProvider.GetRequiredService<IShoppingCartService>())
            { ControllerContext = BuildContext(TestTourist) };

            (cartCtl.AddToCart((int)t1.Id).Result as OkObjectResult).ShouldNotBeNull();
            (cartCtl.AddToCart((int)t2.Id).Result as OkObjectResult).ShouldNotBeNull();

            var checkoutCtl = new CheckoutController(
                scope.ServiceProvider.GetRequiredService<ICheckoutService>())
            { ControllerContext = BuildContext(TestTourist) };

            var ok = checkoutCtl.Checkout().Result as OkObjectResult;
            ok.ShouldNotBeNull();
            var tokens = ok!.Value as List<TourPurchaseTokenDto>;
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
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
            CleanState(db, touristId);

            var tour = MakeTour(db, "Once", 20m);

            
            db.TourPurchaseTokens.Add(new TourPurchaseToken(touristId, (int)tour.Id));
            db.SaveChanges();

            
            var cartCtl = new ShoppingCartController(
                scope.ServiceProvider.GetRequiredService<IShoppingCartService>())
            { ControllerContext = BuildContext(TestTourist) };
            (cartCtl.AddToCart((int)tour.Id).Result as OkObjectResult).ShouldNotBeNull();

            var before = db.TourPurchaseTokens.Count(x => x.TouristId == touristId && x.TourId == tour.Id);

            var checkoutCtl = new CheckoutController(
                scope.ServiceProvider.GetRequiredService<ICheckoutService>())
            { ControllerContext = BuildContext(TestTourist) };
            (checkoutCtl.Checkout().Result as OkObjectResult).ShouldNotBeNull();

            var after = db.TourPurchaseTokens.Count(x => x.TouristId == touristId && x.TourId == tour.Id);
            after.ShouldBe(before); 
        }
    }
}