using Explorer.API.Controllers;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.UseCases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration
{
    [Collection("Sequential")]
    public class TouristLocationControllerTests : BaseStakeholdersIntegrationTest
    {
        public TouristLocationControllerTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Update_and_get_mine_returns_location()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-22");

            var initialAction = controller.GetMine();
            var initialResult = initialAction.Result as OkObjectResult;
            initialResult.ShouldNotBeNull();
            var initial = initialResult!.Value as TouristLocationDto;
            initial.ShouldNotBeNull();
            initial.Latitude.ShouldBe(45.26);
            initial.Longitude.ShouldBe(19.84);

            var dto = new TouristLocationDto { Latitude = 45.251, Longitude = 19.836 };

            var updateAction = controller.Update(dto);
            var updateResult = updateAction.Result as OkObjectResult;
            updateResult.ShouldNotBeNull();
            var updated = updateResult!.Value as TouristLocationDto;
            updated.ShouldNotBeNull();
            updated.Latitude.ShouldBe(45.251);
            updated.Longitude.ShouldBe(19.836);

            var getAction = controller.GetMine();
            var getResult = getAction.Result as OkObjectResult;
            getResult.ShouldNotBeNull();
            var fetched = getResult!.Value as TouristLocationDto;
            fetched.ShouldNotBeNull();
            fetched.Latitude.ShouldBe(45.251);
            fetched.Longitude.ShouldBe(19.836);
        }

        private static TouristLocationController CreateController(IServiceScope scope, string userId)
        {
            return new TouristLocationController(scope.ServiceProvider.GetRequiredService<ITouristLocationService>())
            {
                ControllerContext = BuildContext(userId)
            };
        }
    }
}
