using Explorer.API.Controllers;
using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Stakeholders.Tests.Integration.UserProfiles;

[Collection("Sequential")]
public class UserProfileControllerTests : BaseStakeholdersIntegrationTest
{
    public UserProfileControllerTests(StakeholdersTestFactory factory) : base(factory) { }

    private static ControllerContext BuildContextWithTestAuth(long userId)
    {
        var httpContext = new DefaultHttpContext();

        var claims = new List<Claim>
        {
            new Claim("id", userId.ToString())
        };

        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));

        return new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    private static UserProfileController CreateController(IServiceScope scope, long userId)
    {
        return new UserProfileController(
            scope.ServiceProvider.GetRequiredService<IUserProfileService>(),
            scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>()
        )
        {
            ControllerContext = BuildContextWithTestAuth(userId)
        };
    }

    [Fact]
    public void Gets_profile_after_creating_profile_in_test()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        
        var user = new User("profiletest@example.com", "test123", UserRole.Tourist, true);
        db.Users.Add(user);
        db.SaveChanges();

        
        var person = new Person(user.Id, "Marko", "Marković", "profiletest@example.com");
        db.People.Add(person);
        db.SaveChanges();

        
        var profile = new UserProfile(
            user.Id,
            "Marko",
            "Marković",
            "Bio test",
            "Motto test",
            "test.png"
        );
        db.UserProfiles.Add(profile);
        db.SaveChanges();

        db.ChangeTracker.Clear();

        var controller = CreateController(scope, user.Id);

        
        var action = controller.GetMyProfile();
        var ok = action.Result as OkObjectResult;

        
        ok.ShouldNotBeNull();

        var dto = ok!.Value as UserProfileDto;
        dto.ShouldNotBeNull();

        dto!.UserId.ShouldBe(user.Id);
        dto.FirstName.ShouldBe("Marko");
        dto.LastName.ShouldBe("Marković");
    }

    [Fact]
    public void Updates_own_profile_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        
        var user = new User("updateown@example.com", "test123", UserRole.Tourist, true);
        db.Users.Add(user);
        db.SaveChanges();

        
        var person = new Person(user.Id, "Petar", "Perić", "updateown@example.com");
        db.People.Add(person);

        
        var profile = new UserProfile(
            user.Id, "Petar", "Perić", "Bio", "Motto", "image.png"
        );
        db.UserProfiles.Add(profile);
        db.SaveChanges();
        db.ChangeTracker.Clear();

        var controller = CreateController(scope, user.Id);

        var dto = new UpdateUserProfileDto
        {
            FirstName = "NovoIme",
            LastName = "NoviPrezime",
            Biography = "Novi bio",
            Motto = "Novi motto",
            ProfileImageUrl = "new.png"
        };

        var result = controller.Update(user.Id, dto);
        var ok = result.Result as OkObjectResult;

        ok.ShouldNotBeNull();

        var updated = ok!.Value as UserProfileDto;
        updated.ShouldNotBeNull();

        updated!.FirstName.ShouldBe("NovoIme");
        updated.LastName.ShouldBe("NoviPrezime");
        updated.Biography.ShouldBe("Novi bio");
        updated.Motto.ShouldBe("Novi motto");
        updated.ProfileImageUrl.ShouldBe("new.png");
    }

    [Fact]
    public void Update_forbidden_when_updating_other_users_profile()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        
        var userA = new User("userA@example.com", "pass", UserRole.Tourist, true);
        db.Users.Add(userA);

        
        var userB = new User("userB@example.com", "pass", UserRole.Tourist, true);
        db.Users.Add(userB);

        db.SaveChanges();

        
        var person = new Person(userB.Id, "Ivan", "Ivić", "userB@example.com");
        db.People.Add(person);

        var profile = new UserProfile(
            userB.Id, "Ivan", "Ivić", "Bio", "Motto", "image.png"
        );
        db.UserProfiles.Add(profile);

        db.SaveChanges();
        db.ChangeTracker.Clear();

        var controller = CreateController(scope, userA.Id); 

        var dto = new UpdateUserProfileDto
        {
            FirstName = "Hacker",
            LastName = "Attack",
            Biography = "Trying to change",
            Motto = "Hacked",
            ProfileImageUrl = "hack.png"
        };

        var result = controller.Update(userB.Id, dto);

        var forbid = result.Result as ForbidResult;
        forbid.ShouldNotBeNull();
    }


}
