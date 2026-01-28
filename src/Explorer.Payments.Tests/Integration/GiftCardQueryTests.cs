using System.Collections.Generic;
using System.Linq;
using Explorer.API.Controllers.Tourist.Payments;
using Explorer.API.Hubs;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Payments.Tests.Integration
{
    [Collection("Sequential")]
    public class GiftCardQueryTests : BasePaymentsIntegrationTest
    {
        public GiftCardQueryTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void GetMyGiftCards_returns_cards_for_tourist_with_cards()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateGiftCardController(scope, "-1");

            var result = controller.GetMyGiftCards().Result as OkObjectResult;
            var cards = result?.Value as List<GiftCardDto>;

            result.ShouldNotBeNull();
            cards.ShouldNotBeNull();
            cards.Count.ShouldBeGreaterThanOrEqualTo(1);
            cards.All(c => !string.IsNullOrEmpty(c.Code) && c.Balance > 0).ShouldBeTrue();
        }

        [Fact]
        public void GetMyGiftCards_returns_empty_for_tourist_without_cards()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateGiftCardController(scope, "-99");

            var result = controller.GetMyGiftCards().Result as OkObjectResult;
            var cards = result?.Value as List<GiftCardDto>;

            result.ShouldNotBeNull();
            cards.ShouldNotBeNull();
            cards.Count.ShouldBe(0);
        }

        private static GiftCardController CreateGiftCardController(IServiceScope scope, string touristId)
        {
            return new GiftCardController(
                scope.ServiceProvider.GetRequiredService<IGiftCardService>(),
                scope.ServiceProvider.GetRequiredService<INotificationService>(),
                scope.ServiceProvider.GetRequiredService<IHubContext<MessageHub>>())
            {
                ControllerContext = BuildTouristContext(touristId)
            };
        }
    }
}
