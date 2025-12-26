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
            var cart = db.ShoppingCarts.Include(c => c.Items).FirstOrDefault(c => c.TouristId == touristId);
            if (cart != null) { db.ShoppingCarts.Remove(cart); db.SaveChanges(); }

            var tokens = db.TourPurchaseTokens.Where(t => t.TouristId == touristId).ToList();
            if (tokens.Any()) { db.TourPurchaseTokens.RemoveRange(tokens); db.SaveChanges(); }
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

            
            var checkoutCtl = new CheckoutController(
                scope.ServiceProvider.GetRequiredService<ICheckoutService>())
            {
                ControllerContext = BuildContext(TestTourist)
            };

            var ok = checkoutCtl.Checkout().Result as OkObjectResult;
            ok.ShouldNotBeNull();

            
            var tokens = ok!.Value as List<TourPurchaseTokenDto>;
            tokens.ShouldNotBeNull();
            tokens!.Count.ShouldBe(1);

            var after = db.ShoppingCarts.Include(c => c.Items).First(c => c.TouristId == touristId);
            after.Items.ShouldBeEmpty();
            after.TotalPrice.ShouldBe(0m);
        }
    }
}