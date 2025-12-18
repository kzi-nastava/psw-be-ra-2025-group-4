using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.UseCases.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class TourSearch_Positive_Tests : BaseToursIntegrationTest
    {
        public TourSearch_Positive_Tests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Returns_published_tour_when_keypoint_is_within_radius()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var tour = new Tour("NS Center Walk", "Short city walk",
                TourDifficulty.Easy, 1,
                new() { new TourTransportDuration(60, Explorer.Tours.Core.Domain.TourTransportType.Foot) }, new() { "city" });
            tour.SetStatus(TourStatus.Published);
            tour.AddTourPoint(new TourPoint(0, "Spens", "Hall", 45.246, 19.853, 1, null, null));
            db.Tours.Add(tour);
            db.SaveChanges();

            var svc = scope.ServiceProvider.GetRequiredService<ITourSearchService>();

            var req = new TourSearchRequestDto { Lat = 45.246, Lon = 19.853, RadiusKm = 2 };

            var result = svc.Search(req);

            Assert.NotEmpty(result);
            Assert.Contains(result, r => r.TourId == (int)tour.Id);
            Assert.All(result, r => Assert.True(r.MatchingPoint.DistanceKm <= 2.001));
        }
    }
}