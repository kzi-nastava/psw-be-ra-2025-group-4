using System;
using System.Linq;
using Explorer.API.Controllers.Administrator.Payments;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Administration;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Payments.Tests.Integration
{
    [Collection("Sequential")]
    public class CoinsBundleSaleCommandTests : BasePaymentsIntegrationTest
    {
        public CoinsBundleSaleCommandTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates_sale_successfully()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAdminController(scope);
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            var dto = new CoinsBundleSaleDto
            {
                CoinsBundleId = 1,
                DiscountPercentage = 20m,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7)
            };

            var result = ((ObjectResult)controller.CreateSale(dto).Result)?.Value as CoinsBundleSaleDto;

            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.CoinsBundleId.ShouldBe(1);
            result.DiscountPercentage.ShouldBe(20m);
            result.IsActive.ShouldBeTrue();

            db.CoinsBundleSales.Any(s => s.Id == result.Id).ShouldBeTrue();
        }

        [Fact]
        public void Create_fails_for_invalid_bundle_id()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAdminController(scope);

            var dto = new CoinsBundleSaleDto
            {
                CoinsBundleId = 9999,
                DiscountPercentage = 20m,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7)
            };

            var result = controller.CreateSale(dto).Result;

            result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Create_fails_when_active_sale_exists()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAdminController(scope);

            // Kreiraj prvo sniženje
            var dto1 = new CoinsBundleSaleDto
            {
                CoinsBundleId = 2,
                DiscountPercentage = 20m,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(7)
            };
            controller.CreateSale(dto1);

            // Pokušaj kreirati drugo aktivno sniženje za isti bundle
            var dto2 = new CoinsBundleSaleDto
            {
                CoinsBundleId = 2,
                DiscountPercentage = 30m,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7)
            };

            var result = controller.CreateSale(dto2).Result;

            result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void Deactivates_sale_successfully()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAdminController(scope);
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Kreiraj sniženje
            var createDto = new CoinsBundleSaleDto
            {
                CoinsBundleId = 4,
                DiscountPercentage = 25m,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7)
            };
            var created = ((ObjectResult)controller.CreateSale(createDto).Result)?.Value as CoinsBundleSaleDto;

            var result = controller.DeactivateSale(created!.Id);

            result.ShouldBeOfType<OkResult>();

            var sale = db.CoinsBundleSales.First(s => s.Id == created.Id);
            sale.IsActive.ShouldBeFalse();
        }

        [Fact]
        public void Deletes_sale_successfully()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAdminController(scope);
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Kreiraj sniženje
            var createDto = new CoinsBundleSaleDto
            {
                CoinsBundleId = 5,
                DiscountPercentage = 30m,
                StartDate = DateTime.UtcNow.AddDays(1), // Future sale
                EndDate = DateTime.UtcNow.AddDays(7)
            };
            var created = ((ObjectResult)controller.CreateSale(createDto).Result)?.Value as CoinsBundleSaleDto;

            var result = controller.DeleteSale(created!.Id);

            result.ShouldBeOfType<NoContentResult>();

            db.CoinsBundleSales.Any(s => s.Id == created.Id).ShouldBeFalse();
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