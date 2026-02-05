using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Explorer.API.Controllers.Stakeholders;
using Explorer.Stakeholders.API.Public;
using Shouldly;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration
{
    [Collection("Sequential")]
    public class FollowControllerTests : BaseStakeholdersIntegrationTest
    {
        public FollowControllerTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public async Task FollowUser_returns_forbid_for_administrator()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-1", "Administrator");

            var result = await controller.FollowUser(1);
            result.ShouldBeOfType<ForbidResult>();
        }

        [Fact]
        public async Task UnfollowUser_returns_ok()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11", "Author");

            var res = controller.UnfollowUser(-11);
            res.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public void GetFollowers_returns_list()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11", "Author");

            var res = controller.GetFollowers(-11) as OkObjectResult;
            res.ShouldNotBeNull();
            var users = res!.Value as IEnumerable<Explorer.Stakeholders.API.Dtos.UserAccountDto>;
            users.ShouldNotBeNull();
            users.Count().ShouldBe(1);
            users.First().Id.ShouldBe(-12);
        }

        [Fact]
        public void GetFollowing_returns_list()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11", "Author");

            var res = controller.GetFollowing(-11) as OkObjectResult;
            res.ShouldNotBeNull();
            var users = res!.Value as IEnumerable<Explorer.Stakeholders.API.Dtos.UserAccountDto>;
            users.ShouldNotBeNull();
            users.Count().ShouldBe(1);
            users.First().Id.ShouldBe(-12);
        }

        private static FollowController CreateController(IServiceScope scope, string userId, string role)
        {
            return new FollowController(
                scope.ServiceProvider.GetRequiredService<IFollowService>(),
                scope.ServiceProvider.GetRequiredService<INotificationService>(),
                scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.SignalR.IHubContext<Explorer.API.Hubs.MessageHub>>(),
                scope.ServiceProvider.GetRequiredService<Explorer.Stakeholders.Core.Domain.RepositoryInterfaces.IUserRepository>()
            )
            {
                ControllerContext = BuildContextWithRole(userId, role)
            };
        }

        private static ControllerContext BuildContextWithRole(string userId, string role)
        {
            var httpContext = new DefaultHttpContext();
            var claims = new List<Claim>
            {
                new("id", userId),
                new(ClaimTypes.Role, role)
            };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));

            return new ControllerContext
            {
                HttpContext = httpContext
            };
        }
    }
}
