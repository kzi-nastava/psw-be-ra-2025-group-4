using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class TouristFacilityControllerTests : BaseToursIntegrationTest
    {
        public TouristFacilityControllerTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void GetAll_returns_seeded_facilities()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = new TouristFacilityController(scope.ServiceProvider.GetRequiredService<IFacilityService>())
            {
                ControllerContext = BuildContext("-21")
            };

            var action = controller.GetAll(1, 10);
            var result = action.Result as OkObjectResult;
            result.ShouldNotBeNull();

            var page = result!.Value as PagedResult<FacilityDto>;
            page.ShouldNotBeNull();
            page.Results.Count.ShouldBeGreaterThanOrEqualTo(3);
        }
    }
}
