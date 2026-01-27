using System.Collections.Generic;
using System.Linq;
using Explorer.API.Controllers.Author;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Author;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Integration
{
    [Collection("Sequential")]
    public class AffiliateCodeQueryTests : BasePaymentsIntegrationTest
    {
        public AffiliateCodeQueryTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Retrieves_all_affiliate_codes_for_author()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");

            controller.Create(new CreateAffiliateCodeDto { TourId = null, AffiliateTouristId = -21, Percent = 10 });
            controller.Create(new CreateAffiliateCodeDto { TourId = null, AffiliateTouristId = -22, Percent = 15 });

            var result = ((ObjectResult)controller.GetAll(tourId: null).Result)?.Value as List<AffiliateCodeDto>;

            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);
            result.All(x => x.AuthorId == -11).ShouldBeTrue();
            result.All(x => x.AffiliateTouristId != 0).ShouldBeTrue();
            result.All(x => x.Percent > 0).ShouldBeTrue();
        }

        [Fact]
        public void Filters_by_tour_id()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");

            var result = ((ObjectResult)controller.GetAll(tourId: -2).Result)
                ?.Value as List<AffiliateCodeDto>;

            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result.All(x => x.TourId == -2).ShouldBeTrue();
        }

        [Fact]
        public void Returns_global_affiliate_codes()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");

            var result = ((ObjectResult)controller.GetAll(tourId: null).Result)
                ?.Value as List<AffiliateCodeDto>;

            result.ShouldNotBeNull();
            result.Any(x => x.TourId == null).ShouldBeTrue();
        }

        private static AffiliateCodesController CreateController(IServiceScope scope, string authorId)
        {
            return new AffiliateCodesController(
                scope.ServiceProvider.GetRequiredService<IAffiliateCodeService>())
            {
                ControllerContext = BuildContext(authorId)
            };
        }
    }
}
