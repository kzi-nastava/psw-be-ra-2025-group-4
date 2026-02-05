using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Following
{
    [Collection("Sequential")]
    public class FollowTests : BaseStakeholdersIntegrationTest
    {
        public FollowTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Gets_followers_and_following_from_seed()
        {
            using var scope = Factory.Services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IFollowRepository>();

            repo.Exists(-11, -12).ShouldBeTrue();
            repo.GetFollowerIdsForUser(-12).ShouldContain(-11);
            repo.GetFollowingIdsForUser(-11).ShouldContain(-12);
        }

        [Fact]
        public void Create_and_delete_follow()
        {
            using var scope = Factory.Services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IFollowRepository>();
            var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var follower = new User("repo_follow_1", "pass", UserRole.Tourist, true);
            var followed = new User("repo_follow_2", "pass", UserRole.Tourist, true);
            db.Users.AddRange(follower, followed);
            db.SaveChanges();

            var follow = new Follow(follower.Id, followed.Id);
            repo.Create(follow);

            repo.Exists(follower.Id, followed.Id).ShouldBeTrue();

            repo.Delete(follower.Id, followed.Id);
            repo.Exists(follower.Id, followed.Id).ShouldBeFalse();
        }

        [Fact]
        public void Follow_and_unfollow_create_and_remove_relation()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            var service = scope.ServiceProvider.GetRequiredService<IFollowService>();
            var repo = scope.ServiceProvider.GetRequiredService<IFollowRepository>();

            var follower = new User("service_follow_1", "pass", UserRole.Tourist, true);
            var followed = new User("service_follow_2", "pass", UserRole.Tourist, true);
            db.Users.AddRange(follower, followed);
            db.SaveChanges();

            service.Follow(follower.Id, followed.Id);
            repo.Exists(follower.Id, followed.Id).ShouldBeTrue();

            service.Unfollow(follower.Id, followed.Id);
            repo.Exists(follower.Id, followed.Id).ShouldBeFalse();
        }
    }
}
