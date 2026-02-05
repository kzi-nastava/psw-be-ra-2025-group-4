using System.Linq;
using Explorer.API.Controllers.Tours;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class TourSearchControllerTests : BaseToursIntegrationTest
    {
        public TourSearchControllerTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Search_returns_unprocessable_entity_for_invalid_request()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var result = controller.Search(new TourSearchRequestDto { Lat = 200, Lon = 0, RadiusKm = 1 });
            result.Result.ShouldBeOfType<UnprocessableEntityObjectResult>();
        }

        [Fact]
        public void Search_returns_matching_tours()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var result = controller.Search(new TourSearchRequestDto { Lat = 44.8200, Lon = 20.4530, RadiusKm = 1 });
            result.Result.ShouldBeOfType<OkObjectResult>();
            var ok = result.Result as OkObjectResult;
            var list = ok!.Value as System.Collections.Generic.List<TourSearchResultDto>;
            list.ShouldNotBeNull();
            list!.Count.ShouldBeGreaterThanOrEqualTo(1);
            list.Any(r => r.TourId == -2).ShouldBeTrue();
        }

        private static TourSearchController CreateController(IServiceScope scope)
        {
            return new TourSearchController(scope.ServiceProvider.GetRequiredService<ITourSearchService>());
        }
    }
}
