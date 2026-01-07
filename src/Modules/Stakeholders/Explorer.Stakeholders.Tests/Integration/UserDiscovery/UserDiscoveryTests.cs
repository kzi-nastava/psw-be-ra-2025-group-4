using Explorer.API.Controllers;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Stakeholders.Tests.Integration.UserDiscovery;

[Collection("Sequential")]
public class UserDiscoveryTests : BaseStakeholdersIntegrationTest
{
    public UserDiscoveryTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Get_all_returns_non_admin_users_except_current()
    {
        using var scope = Factory.Services.CreateScope();

        // current user = turista1 (-21) iz seed-a
        var controller = CreateController(scope, "-21");

        var result = controller.Discover(null) as OkObjectResult;
        result.ShouldNotBeNull();

        var users = result!.Value as IEnumerable<UserDiscoveryDto>;
        users.ShouldNotBeNull();

        var list = users!.ToList();

        // Seed:
        // admin (-1) -> ne sme
        // autor1,2,3 (-11,-12,-13)
        // turista2,3 (-22,-23)
        list.Count.ShouldBe(5);

        list.ShouldAllBe(u => u.UserId != -1);   // admin filtered
        list.ShouldAllBe(u => u.UserId != -21);  // current user filtered
    }

    [Fact]
    public void Search_returns_matching_users_only()
    {
        using var scope = Factory.Services.CreateScope();

        var controller = CreateController(scope, "-21");

        var result = controller.Discover("turista") as OkObjectResult;
        result.ShouldNotBeNull();

        var users = result!.Value as IEnumerable<UserDiscoveryDto>;
        users.ShouldNotBeNull();

        var list = users!.ToList();

        list.Count.ShouldBe(2);
        list.ShouldAllBe(u => u.Username.Contains("turista"));
    }

    [Fact]
    public void Search_returns_empty_list_when_no_match()
    {
        using var scope = Factory.Services.CreateScope();

        var controller = CreateController(scope, "-21");

        var result = controller.Discover("nepostoji") as OkObjectResult;
        result.ShouldNotBeNull();

        var users = result!.Value as IEnumerable<UserDiscoveryDto>;
        users.ShouldNotBeNull();

        users!.Count().ShouldBe(0);
    }

    private static UserDiscoveryController CreateController(IServiceScope scope, string userId)
    {
        return new UserDiscoveryController(
            scope.ServiceProvider.GetRequiredService<IUserDiscoveryService>())
        {
            ControllerContext = BuildContext(userId)
        };
    }

    private static ControllerContext BuildContext(string userId)
    {
        var httpContext = new DefaultHttpContext();

        var claims = new List<Claim>
        {
            new Claim("id", userId)
        };

        httpContext.User = new ClaimsPrincipal(
            new ClaimsIdentity(claims, "TestAuth")
        );

        return new ControllerContext
        {
            HttpContext = httpContext
        };
    }
}
