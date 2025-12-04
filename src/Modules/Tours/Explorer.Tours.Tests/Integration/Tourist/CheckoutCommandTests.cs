using System.Linq;
using System.Collections.Generic;
using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Shopping;
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

        [Fact]
        public void Checkout_creates_tokens_and_clears_cart()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Arrange: obezbedi postojanje korpe sa bar jednom stavkom za turistu -20
            // Ako koleginica ne seed-uje korpu, ručno kreiraj preko repo-ja pre poziva kontrolera.

            var controller = CreateController(scope, "-20");

            var action = controller.Checkout().Result as ObjectResult;
            var result = action?.Value as List<TourPurchaseTokenDto>;

            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);

            var firstId = result.First().Id;
            var storedToken = db.TourPurchaseTokens
                                .AsNoTracking()
                                .FirstOrDefault(t => t.Id == firstId);
            storedToken.ShouldNotBeNull();

            // Ako je seed-ovana ShoppingCart tabela, ovde možeš proveriti i da je korpa prazna
            // var cart = db.ShoppingCarts.Include(c => c.Items).First(c => c.TouristId == -20);
            // cart.Items.ShouldBeEmpty();
        }

        private static CheckoutController CreateController(IServiceScope scope, string personId)
        {
            return new CheckoutController(scope.ServiceProvider.GetRequiredService<ICheckoutService>())
            {
                ControllerContext = BuildContext(personId)
            };
        }
    }
}