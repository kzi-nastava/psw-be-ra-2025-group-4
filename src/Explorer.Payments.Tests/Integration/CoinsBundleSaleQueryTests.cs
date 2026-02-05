using System.Collections.Generic;
using System.Linq;
using Explorer.API.Controllers.Administrator.Payments;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Administration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Payments.Tests.Integration
{
    [Collection("Sequential")]
    public class CoinsBundleSaleQueryTests : BasePaymentsIntegrationTest
    {
        public CoinsBundleSaleQueryTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Retrieves_all_sales()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAdminController(scope);

            var result = ((ObjectResult)controller.GetAllSales().Result)?.Value as List<CoinsBundleSaleDto>;

            result.ShouldNotBeNull();
        }

        [Fact]
        public void Retrieves_sale_by_id()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAdminController(scope);

            // Kreiraj sniženje prvo
            var createDto = new CoinsBundleSaleDto
            {
                CoinsBundleId = 3,
                DiscountPercentage = 15m,
                StartDate = System.DateTime.UtcNow,
                EndDate = System.DateTime.UtcNow.AddDays(7)
            };
            var created = ((ObjectResult)controller.CreateSale(createDto).Result)?.Value as CoinsBundleSaleDto;
            created.ShouldNotBeNull("Sale creation should succeed");

            var result = ((ObjectResult)controller.GetSale(created.Id).Result)?.Value as CoinsBundleSaleDto;

            result.ShouldNotBeNull();
            result.Id.ShouldBe(created.Id);
            result.DiscountPercentage.ShouldBe(15m);
        }

        [Fact]
        public void GetSale_returns_not_found_for_invalid_id()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAdminController(scope);

            var result = controller.GetSale(9999).Result;

            result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Retrieves_all_purchases_for_statistics()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAdminController(scope);

            var result = ((ObjectResult)controller.GetAllPurchases().Result)?.Value as List<CoinsBundlePurchaseDto>;

            result.ShouldNotBeNull();
        }

        private static CoinsBundleSaleController CreateAdminController(IServiceScope scope)
        {
            return new CoinsBundleSaleController(
                scope.ServiceProvider.GetRequiredService<ICoinsBundleSaleService>())
            {
                ControllerContext = BuildContext("-1")
            };
        }
    }
}