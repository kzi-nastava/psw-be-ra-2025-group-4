using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.UseCases.Internal;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Explorer.Tours.Tests.Unit
{
    public class BundleInfoServiceTests
    {
        private class BundleRepoStub : IBundleRepository
        {
            public readonly List<Bundle> Store = new();

            public IEnumerable<Bundle> GetAll() => Store;

            public Bundle GetById(int id) => Store.FirstOrDefault(b => b.Id == id);

            public Bundle Create(Bundle b) => throw new NotImplementedException();
            public Bundle Update(Bundle b) => throw new NotImplementedException();
            public void Delete(int id) => throw new NotImplementedException();
            public PagedResult<Bundle> GetPaged(int p, int s) => throw new NotImplementedException();
            public IEnumerable<Bundle> GetByAuthor(int id) => throw new NotImplementedException();
        }

        private static List<Tour> CreateTours(int count)
        {
            var tours = new List<Tour>();
            for (int i = 0; i < count; i++)
            {
                tours.Add(new Tour(
                    id: i + 1,
                    name: $"Tour {i + 1}",
                    description: "desc",
                    difficulty: TourDifficulty.Easy,
                    tags: new List<string> { "tag" },
                    status: TourStatus.Published,
                    authorId: 1,
                    points: new List<TourPoint>(),
                    equipment: new List<Equipment>(),
                    price: 10m,
                    transportDuration: new List<TourTransportDuration>(),
                    publishedAt: DateTime.UtcNow,
                    archivedAt: null,
                    lengthInKm: 0
                ));
            }
            return tours;
        }

        [Fact]
        public void Get_returns_bundle_info_when_bundle_exists_and_status_published()
        {
            var repo = new BundleRepoStub();
            var bundle = new Bundle("B1", 50m, 1, CreateTours(2));
            bundle.Publish(); // Published status
            repo.Store.Add(bundle);

            var service = new BundleInfoService(repo);

            var dto = service.Get((int)bundle.Id);

            dto.ShouldNotBeNull();
            dto.Id.ShouldBe((int)bundle.Id);
            dto.Name.ShouldBe("B1");
            dto.Price.ShouldBe(50m);
            dto.Status.ShouldBe(Explorer.Tours.API.Internal.BundleLifecycleStatus.Published);
            dto.Tours.Count.ShouldBe(2);
            dto.Tours.Select(t => t.Id).ShouldBe(new[] { 1, 2 });
        }

        [Fact]
        public void Get_maps_archived_status()
        {
            var repo = new BundleRepoStub();
            var bundle = new Bundle("B2", 30m, 1, CreateTours(2));
            bundle.Publish();
            bundle.Archive(); // Archived status
            repo.Store.Add(bundle);

            var service = new BundleInfoService(repo);

            var dto = service.Get((int)bundle.Id);

            dto.Status.ShouldBe(Explorer.Tours.API.Internal.BundleLifecycleStatus.Archived);
        }

        [Fact]
        public void Get_maps_draft_status_for_non_published_bundle()
        {
            var repo = new BundleRepoStub();
            var bundle = new Bundle("B3", 20m, 1, CreateTours(2)); // ostaje Draft
            repo.Store.Add(bundle);

            var service = new BundleInfoService(repo);

            var dto = service.Get((int)bundle.Id);

            dto.Status.ShouldBe(Explorer.Tours.API.Internal.BundleLifecycleStatus.Draft);
        }

        [Fact]
        public void Get_throws_not_found_when_bundle_does_not_exist()
        {
            var repo = new BundleRepoStub();
            var service = new BundleInfoService(repo);

            var ex = Should.Throw<NotFoundException>(() => service.Get(123));
            ex.Message.ShouldBe("Bundle 123 not found.");
        }
    }
}
