using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class MysteryTourOfferControllerTests : BaseToursIntegrationTest
    {
        public MysteryTourOfferControllerTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void GetOrCreate_and_redeem_offer()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = new MysteryTourOfferController(scope.ServiceProvider.GetRequiredService<IMysteryTourOfferService>())
            {
                ControllerContext = BuildContext("-90")
            };

            var action = controller.GetOrCreate();
            var result = action.Result as OkObjectResult;
            result.ShouldNotBeNull();

            var offer = result!.Value as MysteryTourOfferDto;
            offer.ShouldNotBeNull();
            offer!.DiscountPercent.ShouldBeGreaterThanOrEqualTo(10);
            offer.DiscountPercent.ShouldBeLessThanOrEqualTo(40);

            var redeemResult = controller.Redeem(offer.Id) as NoContentResult;
            redeemResult.ShouldNotBeNull();
        }
    }
}
