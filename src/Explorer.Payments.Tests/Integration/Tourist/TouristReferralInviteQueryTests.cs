using Explorer.API.Controllers.Tourist;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Dtos.Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace Explorer.Payments.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class TouristReferralInviteQueryTests : BasePaymentsIntegrationTest
    {
        public TouristReferralInviteQueryTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Create_returns_ok_and_persists_invite_for_current_tourist()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            const string touristId = "-21";

            var controller = CreateController(scope, touristId);

            var result = controller.Create();
            var ok = result.Result as OkObjectResult;

            ok.ShouldNotBeNull();
            var dto = ok!.Value as TouristReferralInviteDto;

            dto.ShouldNotBeNull();
            dto!.Code.ShouldNotBeNullOrWhiteSpace();
            dto.IsUsed.ShouldBeFalse();

            db.ChangeTracker.Clear();
            db.TouristReferralInvites.Any(x =>
                    x.Code == dto.Code &&
                    x.ReferrerTouristId == long.Parse(touristId))
                .ShouldBeTrue();
        }

        private static TouristReferralInviteController CreateController(IServiceScope scope, string touristId)
        {
            return new TouristReferralInviteController(
                scope.ServiceProvider.GetRequiredService<ITouristReferralInviteService>())
            {
                ControllerContext = BuildTouristContext(touristId)
            };
        }
    }
}
