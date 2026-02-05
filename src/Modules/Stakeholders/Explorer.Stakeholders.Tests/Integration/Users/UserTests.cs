using System.Linq;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Users
{
    [Collection("Sequential")]
    public class UserTests : BaseStakeholdersIntegrationTest
    {
        public UserTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Gets_users_from_seed()
        {
            using var scope = Factory.Services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            repo.Exists("autor1@gmail.com").ShouldBeTrue();
            repo.GetActiveByName("autor1@gmail.com").ShouldNotBeNull();
            repo.GetPersonId(-11).ShouldBe(-11);
            repo.GetTourists(true).Count().ShouldBe(3);

            var paged = repo.GetPaged(1, 10);
            paged.TotalCount.ShouldBeGreaterThanOrEqualTo(7);
        }

        [Fact]
        public void GetById_returns_user()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IUserService>();

            var user = service.GetById(-11);
            user.ShouldNotBeNull();
            user!.Username.ShouldBe("autor1@gmail.com");
        }
    }
}
