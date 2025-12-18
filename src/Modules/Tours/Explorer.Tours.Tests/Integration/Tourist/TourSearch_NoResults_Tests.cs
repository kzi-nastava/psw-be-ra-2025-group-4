using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class TourSearch_NoResults_Tests : BaseToursIntegrationTest
    {
        public TourSearch_NoResults_Tests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Returns_empty_when_no_keypoints_in_radius()
        {
            using var scope = Factory.Services.CreateScope();
            var svc = scope.ServiceProvider.GetRequiredService<ITourSearchService>();

            var req = new TourSearchRequestDto { Lat = 44.0, Lon = 20.0, RadiusKm = 1 };

            var res = svc.Search(req);

            Assert.Empty(res);
        }
    }
}