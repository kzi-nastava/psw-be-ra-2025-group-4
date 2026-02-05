using System;
using System.Linq;
using Explorer.API.Controllers.Author;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Author;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Payments.Tests.Integration.Author
{
    [Collection("Sequential")]
    public class AffiliateStatsControllerTests : BasePaymentsIntegrationTest
    {
        public AffiliateStatsControllerTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Summary_returns_values_from_redemptions()
        {
            using var scope = Factory.Services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IAffiliateRedemptionRepository>();

            repo.Create(new AffiliateRedemption(
                affiliateCodeId: 1,
                code: "AFFTEST",
                authorId: 1,
                tourId: 2,
                affiliateTouristId: 21,
                buyerTouristId: 22,
                amountPaid: 100m,
                commissionAmount: 10m));

            var controller = CreateController(scope, "1");

            var action = controller.Summary();
            var result = action.Result as OkObjectResult;
            result.ShouldNotBeNull();

            var summary = result!.Value as AffiliateDashboardSummaryDto;
            summary.ShouldNotBeNull();
            summary!.TotalUsages.ShouldBeGreaterThanOrEqualTo(1);
            summary.TotalRevenue.ShouldBeGreaterThanOrEqualTo(100m);
            summary.TotalCost.ShouldBeGreaterThanOrEqualTo(10m);
        }

        [Fact]
        public void ByTour_returns_stats_grouped_by_tour()
        {
            using var scope = Factory.Services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IAffiliateRedemptionRepository>();

            repo.Create(new AffiliateRedemption(
                affiliateCodeId: 2,
                code: "AFFTEST2",
                authorId: 1,
                tourId: 3,
                affiliateTouristId: 21,
                buyerTouristId: 23,
                amountPaid: 50m,
                commissionAmount: 5m));

            var controller = CreateController(scope, "1");

            var action = controller.ByTour();
            var result = action.Result as OkObjectResult;
            result.ShouldNotBeNull();

            var stats = result!.Value as List<AffiliateTourStatsDto>;
            stats.ShouldNotBeNull();
            stats.Any(s => s.TourId == 3).ShouldBeTrue();
        }

        private static AffiliateStatsController CreateController(IServiceScope scope, string authorId)
        {
            var controller = new AffiliateStatsController(scope.ServiceProvider.GetRequiredService<IAffiliateStatsService>())
            {
                ControllerContext = BuildContext(authorId)
            };

            return controller;
        }
    }
}
