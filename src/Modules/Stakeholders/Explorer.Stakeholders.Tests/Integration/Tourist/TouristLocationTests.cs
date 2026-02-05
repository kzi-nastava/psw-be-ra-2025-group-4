using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class TouristLocationTests : BaseStakeholdersIntegrationTest
    {
        public TouristLocationTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void GetById_returns_seeded_location()
        {
            using var scope = Factory.Services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ITouristLocationRepository>();

            var location = repo.GetById(-22);
            location.ShouldNotBeNull();
            location!.Latitude.ShouldBe(45.26);
            location.Longitude.ShouldBe(19.84);
        }

        [Fact]
        public void Save_creates_and_updates_location()
        {
            using var scope = Factory.Services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ITouristLocationRepository>();
            var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var user = new User("repo_location", "pass", UserRole.Tourist, true);
            db.Users.Add(user);
            db.SaveChanges();

            var created = repo.Save(new TouristLocation(user.Id, 45.1, 19.1));
            created.UserId.ShouldBe(user.Id);
            created.Latitude.ShouldBe(45.1);

            var updated = repo.Save(new TouristLocation(user.Id, 45.2, 19.2));
            updated.UserId.ShouldBe(user.Id);
            updated.Latitude.ShouldBe(45.2);
            updated.Longitude.ShouldBe(19.2);
        }

        [Fact]
        public void SaveOrUpdate_and_get_location()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            var service = scope.ServiceProvider.GetRequiredService<ITouristLocationService>();

            var user = new User("service_location", "pass", UserRole.Tourist, true);
            db.Users.Add(user);
            db.SaveChanges();

            var created = service.SaveOrUpdateLocation(user.Id, new TouristLocationDto { Latitude = 45.0, Longitude = 19.0 });
            created.Latitude.ShouldBe(45.0);

            var updated = service.SaveOrUpdateLocation(user.Id, new TouristLocationDto { Latitude = 45.5, Longitude = 19.5 });
            updated.Latitude.ShouldBe(45.5);

            var fetched = service.Get(user.Id);
            fetched.Latitude.ShouldBe(45.5);
            fetched.Longitude.ShouldBe(19.5);
        }
    }
}
