using System;
using System.Linq;
using Explorer.API.Controllers.Author;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Author;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Integration
{
    [Collection("Sequential")]
    public class AffiliateCodeCommandTests : BasePaymentsIntegrationTest
    {
        public AffiliateCodeCommandTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates_affiliate_code_for_all_tours()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            var dto = new CreateAffiliateCodeDto
            {
                TourId = null,
                AffiliateTouristId = -21,
                Percent = 10,
                ExpiresAt = null
            };

            var result = ((ObjectResult)controller.Create(dto).Result)?.Value as AffiliateCodeDto;

            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.Code.ShouldNotBeNullOrEmpty();
            result.AuthorId.ShouldBe(-11);
            result.TourId.ShouldBeNull();
            result.Active.ShouldBeTrue();
            result.AffiliateTouristId.ShouldBe(-21);
            result.Percent.ShouldBe(10);
            result.ExpiresAt.ShouldBeNull();
            result.UsageCount.ShouldBe(0);

            db.AffiliateCodes.Any(x => x.Id == result.Id).ShouldBeTrue();
        }

        [Fact]
        public void Create_generates_unique_readable_code()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");

            var dto = new CreateAffiliateCodeDto
            {
                TourId = null,
                AffiliateTouristId = -21,
                Percent = 10
            };

            var result = ((ObjectResult)controller.Create(dto).Result)?.Value as AffiliateCodeDto;

            result.ShouldNotBeNull();
            result.Code.Length.ShouldBe(10);
            result.Code.ShouldMatch(@"^[A-Z2-9]+$");
        }

        [Fact]
        public void Creates_code_with_expiration()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");

            var expires = DateTime.UtcNow.AddDays(7);

            var dto = new CreateAffiliateCodeDto
            {
                TourId = null,
                AffiliateTouristId = -21,
                Percent = 10,
                ExpiresAt = expires
            };

            var result = ((ObjectResult)controller.Create(dto).Result)?.Value as AffiliateCodeDto;

            result.ShouldNotBeNull();
            result.ExpiresAt.ShouldNotBeNull();
            result.ExpiresAt.Value.ShouldBeInRange(expires.AddMinutes(-1), expires.AddMinutes(1));
        }

        [Fact]
        public void Cannot_create_with_past_expiration()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");

            var dto = new CreateAffiliateCodeDto
            {
                TourId = null,
                AffiliateTouristId = -21,
                Percent = 10,
                ExpiresAt = DateTime.UtcNow.AddMinutes(-5)
            };

            Should.Throw<Exception>(() => controller.Create(dto));
        }

        [Fact]
        public void Deactivates_affiliate_code()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            var dto = new CreateAffiliateCodeDto
            {
                TourId = null,
                AffiliateTouristId = -21,
                Percent = 10
            };

            var created = ((ObjectResult)controller.Create(dto).Result)?.Value as AffiliateCodeDto;
            created.ShouldNotBeNull();

            var deleteResult = controller.Deactivate(created.Id);
            deleteResult.ShouldBeOfType<NoContentResult>();

            var entity = db.AffiliateCodes.First(x => x.Id == created.Id);
            entity.Active.ShouldBeFalse();
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
