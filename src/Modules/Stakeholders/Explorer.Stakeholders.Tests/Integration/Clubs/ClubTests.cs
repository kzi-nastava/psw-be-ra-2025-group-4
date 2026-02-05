using System.Linq;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Clubs
{
    [Collection("Sequential")]
    public class ClubTests : BaseStakeholdersIntegrationTest
    {
        public ClubTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void GetAll_and_get_by_owner_return_seeded()
        {
            using var scope = Factory.Services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IClubRepository>();

            var all = repo.GetAll().ToList();
            all.Count.ShouldBeGreaterThanOrEqualTo(2);

            var owned = repo.GetByOwner(-21).ToList();
            owned.Count.ShouldBeGreaterThanOrEqualTo(2);
        }

        [Fact]
        public void Create_update_and_delete_club_repository()
        {
            using var scope = Factory.Services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IClubRepository>();

            var club = new Club("Repo Club", "Repo desc", -21, new System.Collections.Generic.List<string> { "img" });
            var created = repo.Create(club);

            created.Id.ShouldNotBe(0);

            created.Update("Repo Club Updated", "Repo desc", created.ImageUrls);
            var updated = repo.Update(created);
            updated.Name.ShouldBe("Repo Club Updated");

            repo.Delete(created.Id);
            repo.GetAll().Any(c => c.Id == created.Id).ShouldBeFalse();
        }

        [Fact]
        public void Create_update_and_delete_club_service()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IClubService>();

            var createDto = new ClubDto
            {
                Name = "Service Club",
                Description = "Service desc",
                OwnerId = -21,
                ImageUrls = new System.Collections.Generic.List<string> { "img" }
            };

            var created = service.Create(createDto);
            created.Id.ShouldNotBe(0);

            created.Name = "Service Club Updated";
            var updated = service.Update(created.Id, created);
            updated.Name.ShouldBe("Service Club Updated");

            service.Delete(created.Id, -21);
            service.GetByOwner(-21).Any(c => c.Id == created.Id).ShouldBeFalse();
        }
    }
}
