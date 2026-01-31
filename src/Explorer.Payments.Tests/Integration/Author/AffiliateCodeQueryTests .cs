using System.Collections.Generic;
using System.Linq;
using Explorer.API.Controllers.Author;
using Explorer.Payments.API.Dtos;
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

            controller.Create(new CreateAffiliateCodeDto { TourId = null, AffiliateTouristId = -21, Percent = 10 })
                .GetAwaiter().GetResult();
            controller.Create(new CreateAffiliateCodeDto { TourId = null, AffiliateTouristId = -22, Percent = 15 })
                .GetAwaiter().GetResult();

            var result = ExtractOk(controller.GetAll(tourId: null));

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

            var c1 = ExtractCreated(controller.Create(new CreateAffiliateCodeDto { TourId = -2, AffiliateTouristId = -21, Percent = 10 }));
            var c2 = ExtractCreated(controller.Create(new CreateAffiliateCodeDto { TourId = -2, AffiliateTouristId = -22, Percent = 15 }));
            controller.Create(new CreateAffiliateCodeDto { TourId = null, AffiliateTouristId = -23, Percent = 12 })
                .GetAwaiter().GetResult();

            var result = ExtractOk(controller.GetAll(tourId: -2));

            result.ShouldNotBeNull();
            result.All(x => x.TourId == -2).ShouldBeTrue();

            // robust: baza nije prazna, ali ova 2 moraju da postoje
            result.Any(x => x.Id == c1.Id).ShouldBeTrue();
            result.Any(x => x.Id == c2.Id).ShouldBeTrue();
        }

        [Fact]
        public void Returns_global_affiliate_codes()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");

            controller.Create(new CreateAffiliateCodeDto { TourId = null, AffiliateTouristId = -21, Percent = 10 })
                .GetAwaiter().GetResult();

            var result = ExtractOk(controller.GetAll(tourId: null));

            result.ShouldNotBeNull();
            result.Any(x => x.TourId == null).ShouldBeTrue();
        }

        private static AffiliateCodesController CreateController(IServiceScope scope, string authorId)
        {
            var controller = ActivatorUtilities.CreateInstance<AffiliateCodesController>(scope.ServiceProvider);
            controller.ControllerContext = BuildContext(authorId);
            return controller;
        }

        private static List<AffiliateCodeDto> ExtractOk(ActionResult<List<AffiliateCodeDto>> action)
        {
            var ok = action.Result as OkObjectResult;
            ok.ShouldNotBeNull("Expected OkObjectResult.");
            var list = ok.Value as List<AffiliateCodeDto>;
            list.ShouldNotBeNull("Expected OkObjectResult.Value to be List<AffiliateCodeDto>.");
            return list;
        }

        private static AffiliateCodeDto ExtractCreated(System.Threading.Tasks.Task<ActionResult<AffiliateCodeDto>> task)
        {
            var action = task.GetAwaiter().GetResult();

            var created = action.Result as CreatedResult;
            created.ShouldNotBeNull("Expected CreatedResult.");
            var dto = created.Value as AffiliateCodeDto;
            dto.ShouldNotBeNull("Expected CreatedResult.Value to be AffiliateCodeDto.");
            return dto;
        }
    }
}
