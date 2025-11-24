using System.Collections.Generic;
using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Stakeholders.Tests.Integration.Ratings;

[Collection("Sequential")]
public class RatingControllerTests : BaseStakeholdersIntegrationTest
{
    public RatingControllerTests(StakeholdersTestFactory factory) : base(factory) { }

    private ControllerContext FakeUser(long userId, string role)
    {
        var ctx = new DefaultHttpContext();
        ctx.User = new ClaimsPrincipal(
            new ClaimsIdentity(new[]
            {
                new Claim("id", userId.ToString()),
                new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", role)
            })
        );
        return new ControllerContext { HttpContext = ctx };
    }

    private RatingController CreateController(IServiceScope scope, long userId, string role)
    {
        return new RatingController(scope.ServiceProvider.GetRequiredService<IRatingService>())
        {
            ControllerContext = FakeUser(userId, role)
        };
    }

    [Fact]
    public void Admin_can_get_all_ratings()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        db.Ratings.Add(new Rating(101, 5, "Test 1"));
        db.Ratings.Add(new Rating(102, 3, "Test 2"));
        db.SaveChanges();

        var controller = CreateController(scope, 1, "administrator");

        var response = controller.GetAll().Result as OkObjectResult;

        response.ShouldNotBeNull();

        var list = response!.Value as List<RatingDto>;
        list.ShouldNotBeNull();
        list!.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Tourist_can_get_his_own_ratings()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        db.Ratings.Add(new Rating(101, 4, "My rating"));
        db.Ratings.Add(new Rating(102, 1, "Not mine"));
        db.SaveChanges();

        var controller = CreateController(scope, 101, "tourist");

        var result = controller.GetMine().Result as OkObjectResult;

        result.ShouldNotBeNull();

        var list = result!.Value as List<RatingDto>;
        list.ShouldNotBeNull();
        list!.ShouldAllBe(r => r.UserId == 101);
    }

    [Fact]
    public void Tourist_can_create_rating()
    {
        using var scope = Factory.Services.CreateScope();

        var controller = CreateController(scope, 101, "tourist");

        var dto = new RatingCreateDto
        {
            Value = 4,
            Comment = "Test komentar"
        };

        var result = controller.Create(dto).Result as ObjectResult;

        result.ShouldNotBeNull();
        result!.StatusCode.ShouldBe(201);

        var rating = result.Value as RatingDto;
        rating.ShouldNotBeNull();
        rating!.Value.ShouldBe(4);
        rating.Comment.ShouldBe("Test komentar");
        rating.UserId.ShouldBe(101);
    }

    [Fact]
    public void Tourist_cannot_update_others_rating()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var foreign = new Rating(102, 3, "Not yours");
        db.Ratings.Add(foreign);
        db.SaveChanges();

        var controller = CreateController(scope, 101, "tourist");

        var dto = new RatingUpdateDto
        {
            Value = 1,
            Comment = "Ne bi smelo da radi"
        };

        Should.Throw<UnauthorizedAccessException>(() =>
        {
            controller.Update(foreign.Id, dto);
        });
    }

    [Fact]
    public void Tourist_can_update_his_own_rating()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var rating = new Rating(101, 5, "Old");
        db.Ratings.Add(rating);
        db.SaveChanges();

        var controller = CreateController(scope, 101, "tourist");

        var dto = new RatingUpdateDto
        {
            Value = 5,
            Comment = "Updated"
        };

        var result = controller.Update(rating.Id, dto).Result as OkObjectResult;

        result.ShouldNotBeNull();

        var updated = result!.Value as RatingDto;
        updated.ShouldNotBeNull();
        updated!.Comment.ShouldBe("Updated");
    }

    [Fact]
    public void Tourist_cannot_delete_others_rating()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var rating = new Rating(102, 5, "Not yours");
        db.Ratings.Add(rating);
        db.SaveChanges();

        var controller = CreateController(scope, 101, "tourist");

        Should.Throw<UnauthorizedAccessException>(() =>
        {
            controller.Delete(rating.Id);
        });
    }

    [Fact]
    public void Tourist_can_delete_his_own_rating()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var rating = new Rating(101, 5, "Delete");
        db.Ratings.Add(rating);
        db.SaveChanges();

        var controller = CreateController(scope, 101, "tourist");

        var result = controller.Delete(rating.Id) as StatusCodeResult;

        result.ShouldNotBeNull();
        result!.StatusCode.ShouldBe(204);
    }
}
