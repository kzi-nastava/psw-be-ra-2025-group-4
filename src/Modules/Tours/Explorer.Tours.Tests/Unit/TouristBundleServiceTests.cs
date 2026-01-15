using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.UseCases.Tourist;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Tests.Unit
{
    public class TouristBundleServiceTests
    {
        private class BundleRepoStub : IBundleRepository
        {
            public List<Bundle> Bundles { get; set; } = new();

            public IEnumerable<Bundle> GetAll() => Bundles;

            public PagedResult<Bundle> GetPaged(int p, int s) => throw new NotImplementedException();
            public Bundle GetById(int id) => Bundles.FirstOrDefault(b => b.Id == id) ?? throw new KeyNotFoundException();
            public Bundle Create(Bundle b) => throw new NotImplementedException();
            public Bundle Update(Bundle b) => throw new NotImplementedException();
            public void Delete(int id) => throw new NotImplementedException();
            public IEnumerable<Bundle> GetByAuthor(int id) => throw new NotImplementedException();
        }

        private static IMapper Mapper()
        {
            var cfg = new MapperConfiguration(c =>
            {
                c.CreateMap<Tour, TourDto>();
            });
            return cfg.CreateMapper();
        }

        private static List<Tour> CreatePublishedTours(int count)
        {
            var tours = new List<Tour>();
            for (int i = 0; i < count; i++)
            {
                var tour = new Tour(
                    id: i + 1,
                    name: $"Tour {i + 1}",
                    description: "Description",
                    difficulty: TourDifficulty.Easy,
                    tags: new List<string> { "tag" },
                    status: TourStatus.Published,
                    authorId: 1,
                    points: new List<TourPoint>(),
                    equipment: new List<Equipment>(),
                    price: 10.00m,
                    transportDuration: new List<TourTransportDuration>(),
                    publishedAt: DateTime.UtcNow,
                    archivedAt: null,
                    lengthInKm: 0
                );
                tours.Add(tour);
            }
            return tours;
        }

        [Fact]
        public void GetPublished_returns_only_published_bundles()
        {
            var repo = new BundleRepoStub();
            var publishedTours = CreatePublishedTours(2);
            
            var publishedBundle = new Bundle("Published Bundle", 50.00m, 1, publishedTours);
            publishedBundle.Publish();
            
            var draftBundle = new Bundle("Draft Bundle", 30.00m, 1, publishedTours);
            
            repo.Bundles.AddRange(new[] { publishedBundle, draftBundle });

            var service = new TouristBundleService(repo, Mapper());
            var result = service.GetPublished(1, 10);

            result.Results.Count.ShouldBe(1);
            result.Results[0].Name.ShouldBe("Published Bundle");
            result.Results[0].Status.ShouldBe(BundleDtoStatus.Published);
        }

        [Fact]
        public void GetPublished_returns_correct_pagination()
        {
            var repo = new BundleRepoStub();
            var publishedTours = CreatePublishedTours(2);
            
            var bundle1 = new Bundle("Bundle 1", 50.00m, 1, publishedTours);
            bundle1.Publish();
            var bundle2 = new Bundle("Bundle 2", 40.00m, 1, publishedTours);
            bundle2.Publish();
            var bundle3 = new Bundle("Bundle 3", 30.00m, 1, publishedTours);
            bundle3.Publish();
            
            repo.Bundles.AddRange(new[] { bundle1, bundle2, bundle3 });

            var service = new TouristBundleService(repo, Mapper());
            var result = service.GetPublished(1, 2);

            result.Results.Count.ShouldBe(2);
            result.TotalCount.ShouldBe(3);
        }

        [Fact]
        public void GetPublished_returns_empty_when_no_published_bundles()
        {
            var repo = new BundleRepoStub();
            var publishedTours = CreatePublishedTours(2);
            
            var draftBundle = new Bundle("Draft Bundle", 30.00m, 1, publishedTours);
            repo.Bundles.Add(draftBundle);

            var service = new TouristBundleService(repo, Mapper());
            var result = service.GetPublished(1, 10);

            result.Results.Count.ShouldBe(0);
            result.TotalCount.ShouldBe(0);
        }

        [Fact]
        public void GetById_returns_bundle_when_published()
        {
            var repo = new BundleRepoStub();
            var publishedTours = CreatePublishedTours(2);
            
            var bundle = new Bundle("Test Bundle", 50.00m, 1, publishedTours);
            bundle.Publish();
            repo.Bundles.Add(bundle);

            var service = new TouristBundleService(repo, Mapper());
            var result = service.GetById((int)bundle.Id);

            result.ShouldNotBeNull();
            result.Name.ShouldBe("Test Bundle");
            result.Status.ShouldBe(BundleDtoStatus.Published);
            result.Tours.Count.ShouldBe(2);
        }

        [Fact]
        public void GetById_throws_when_bundle_not_published()
        {
            var repo = new BundleRepoStub();
            var publishedTours = CreatePublishedTours(2);
            
            var bundle = new Bundle("Draft Bundle", 30.00m, 1, publishedTours);
            repo.Bundles.Add(bundle);

            var service = new TouristBundleService(repo, Mapper());
            
            Should.Throw<ArgumentException>(() => service.GetById((int)bundle.Id))
                .Message.ShouldBe("Bundle is not published.");
        }

        [Fact]
        public void GetById_throws_when_bundle_not_found()
        {
            var repo = new BundleRepoStub();
            var service = new TouristBundleService(repo, Mapper());
            
            Should.Throw<KeyNotFoundException>(() => service.GetById(999));
        }
    }
}


