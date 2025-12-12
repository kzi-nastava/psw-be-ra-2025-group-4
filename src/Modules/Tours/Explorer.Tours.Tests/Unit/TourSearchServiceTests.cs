using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.UseCases.Tourist;
using Shouldly;

namespace Explorer.Tours.Tests.Unit
{
    public class TourSearchServiceTests
    {
        private class TourRepoStub : ITourRepository
        {
            public List<Tour> Published { get; set; } = new();

            public IEnumerable<Tour> GetPublished() => Published;

            public PagedResult<Tour> GetPaged(int p, int s) => throw new NotImplementedException();
            public Tour GetById(int id) => throw new NotImplementedException();
            public Tour Create(Tour t) => throw new NotImplementedException();
            public Tour Update(Tour t) => throw new NotImplementedException();
            public void Delete(int id) => throw new NotImplementedException();
            public IEnumerable<Tour> GetByAuthor(int id) => throw new NotImplementedException();
            public IEnumerable<Tour> GetPublishedAndArchived() => throw new NotImplementedException();
        }

        [Fact]
        public void Filters_out_tours_outside_radius()
        {
            var repo = new TourRepoStub();

            var tNear = new Tour("Near", "desc", TourDifficulty.Easy, 1,
                new List<TourTransportDuration> { new TourTransportDuration(30, Explorer.Tours.Core.Domain.TourTransportType.Foot) },
                new List<string> { "tag" });

            tNear.SetStatus(TourStatus.Published);
            tNear.AddTourPoint(new TourPoint(0, "KP1", "d", 45.246, 19.853, 1, null, null));

            var tFar = new Tour("Far", "desc", TourDifficulty.Easy, 1,
                new List<TourTransportDuration> { new TourTransportDuration(30, Explorer.Tours.Core.Domain.TourTransportType.Foot) },
                new List<string> { "tag" });

            tFar.SetStatus(TourStatus.Published);
            tFar.AddTourPoint(new TourPoint(0, "KP2", "d", 44.0, 20.0, 1, null, null));

            repo.Published.AddRange(new[] { tNear, tFar });

            var service = new TourSearchService(repo);

            var req = new TourSearchRequestDto { Lat = 45.246, Lon = 19.853, RadiusKm = 2 };

            var res = service.Search(req);

            res.Count.ShouldBe(1);
            res[0].Name.ShouldBe("Near");
        }
    }
}
