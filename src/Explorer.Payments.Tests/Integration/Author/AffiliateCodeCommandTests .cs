using System;
using System.Linq;
using System.Threading.Tasks;
using Explorer.API.Controllers.Author;
using Explorer.Payments.API.Dtos;
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
        public async Task Creates_affiliate_code_for_all_tours()
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

            var result = await ExtractCreatedAsync(controller.Create(dto));

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
        public async Task Create_generates_unique_readable_code()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");

            var dto = new CreateAffiliateCodeDto
            {
                TourId = null,
                AffiliateTouristId = -21,
                Percent = 10
            };

            var result = await ExtractCreatedAsync(controller.Create(dto));

            result.ShouldNotBeNull();
            result.Code.Length.ShouldBe(10);
            result.Code.ShouldMatch(@"^[A-Z2-9]+$");
        }

        [Fact]
        public async Task Creates_code_with_expiration()
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

            var result = await ExtractCreatedAsync(controller.Create(dto));

            result.ShouldNotBeNull();
            result.ExpiresAt.ShouldNotBeNull();
            result.ExpiresAt!.Value.ShouldBeInRange(expires.AddMinutes(-1), expires.AddMinutes(1));
        }

        [Fact]
        public async Task Cannot_create_with_past_expiration()
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

            await Should.ThrowAsync<ArgumentException>(async () =>
            {
                await controller.Create(dto);
            });
        }

        [Fact]
        public async Task Deactivates_affiliate_code()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            var created = await ExtractCreatedAsync(controller.Create(new CreateAffiliateCodeDto
            {
                TourId = null,
                AffiliateTouristId = -21,
                Percent = 10
            }));

            created.ShouldNotBeNull();

            var deleteResult = controller.Deactivate(created.Id);
            deleteResult.ShouldBeOfType<NoContentResult>();

            var entity = db.AffiliateCodes.First(x => x.Id == created.Id);
            entity.Active.ShouldBeFalse();
        }

        private static AffiliateCodesController CreateController(IServiceScope scope, string authorId)
        {
            var controller = ActivatorUtilities.CreateInstance<AffiliateCodesController>(scope.ServiceProvider);
            controller.ControllerContext = BuildContext(authorId);
            return controller;
        }

        private static async Task<AffiliateCodeDto> ExtractCreatedAsync(Task<ActionResult<AffiliateCodeDto>> actionTask)
        {
            var action = await actionTask;

            if (action.Result is CreatedResult created)
            {
                var dto = created.Value as AffiliateCodeDto;
                dto.ShouldNotBeNull("Expected CreatedResult.Value to be AffiliateCodeDto.");
                return dto;
            }

            if (action.Result is OkObjectResult ok)
            {
                var dto = ok.Value as AffiliateCodeDto;
                dto.ShouldNotBeNull("Expected OkObjectResult.Value to be AffiliateCodeDto.");
                return dto;
            }

            if (action.Value != null)
            {
                return action.Value;
            }

            throw new Exception($"Unexpected result type: {action.Result?.GetType().Name ?? "null"}");
        }
    }
}
