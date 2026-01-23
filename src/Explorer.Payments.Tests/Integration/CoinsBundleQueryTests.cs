using System.Collections.Generic;
using System.Linq;
using Explorer.API.Controllers.Tourist.Payments;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Payments.Tests.Integration
{
    [Collection("Sequential")]
    public class CoinsBundleQueryTests : BasePaymentsIntegrationTest
    {
        public CoinsBundleQueryTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Retrieves_all_bundles()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-1");

            var result = ((ObjectResult)controller.GetAllBundles().Result)?.Value as List<CoinsBundleDto>;

            result.ShouldNotBeNull();
            result.Count.ShouldBe(6);
            result.All(b => b.Price > 0).ShouldBeTrue();
            result.All(b => b.CoinsAmount > 0).ShouldBeTrue();
        }

        [Fact]
        public void Bundles_are_ordered_by_display_order()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-1");

            var result = ((ObjectResult)controller.GetAllBundles().Result)?.Value as List<CoinsBundleDto>;

            result.ShouldNotBeNull();
            for (int i = 0; i < result.Count - 1; i++)
            {
                result[i].DisplayOrder.ShouldBeLessThan(result[i + 1].DisplayOrder);
            }
        }

        [Fact]
        public void Retrieves_bundle_by_id()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-1");

            var result = ((ObjectResult)controller.GetBundle(1).Result)?.Value as CoinsBundleDto;

            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Name.ShouldBe("Starter Pack");
            result.CoinsAmount.ShouldBe(500);
            result.BonusCoins.ShouldBe(0);
            result.TotalCoins.ShouldBe(500);
            result.Price.ShouldBe(5.00m);
        }

        [Fact]
        public void GetBundle_returns_not_found_for_invalid_id()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-1");

            var result = controller.GetBundle(9999).Result;

            result.ShouldBeOfType<NotFoundObjectResult>();
        }
        [Fact]
        public void Bundle_shows_sale_information_when_active_sale_exists()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-1");

            var result = ((ObjectResult)controller.GetBundle(2).Result)?.Value as CoinsBundleDto;

            result.ShouldNotBeNull();
            if (result.IsOnSale)
            {
                result.DiscountPercentage.ShouldNotBeNull();
                result.DiscountedPrice.ShouldNotBeNull();
                result.DiscountedPrice.Value.ShouldBeLessThan(result.Price);  
            }
        }

        [Fact]
        public void Retrieves_purchase_history_for_tourist()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-1");

            var result = ((ObjectResult)controller.GetPurchaseHistory().Result)?.Value as List<CoinsBundlePurchaseDto>;

            result.ShouldNotBeNull();
            result.All(p => p.TouristId == -1).ShouldBeTrue();
        }

        private static CoinsBundleController CreateTouristController(IServiceScope scope, string touristId)
        {
            return new CoinsBundleController(
                scope.ServiceProvider.GetRequiredService<ICoinsBundleService>())
            {
                ControllerContext = BuildContext(touristId)
            };
        }
    }
}