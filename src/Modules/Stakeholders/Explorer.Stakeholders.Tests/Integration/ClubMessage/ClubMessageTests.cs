using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Explorer.API.Controllers.Tourist;
using Explorer.API.Hubs;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration.Clubs;

[Collection("Sequential")]
public class ClubMessageControllerTests : BaseStakeholdersIntegrationTest
{
    public ClubMessageControllerTests(StakeholdersTestFactory factory) : base(factory) { }

    private ControllerContext FakeUser(long userId)
    {
        var ctx = new DefaultHttpContext();
        ctx.User = new ClaimsPrincipal(
            new ClaimsIdentity(new[]
            {
                new Claim("id", userId.ToString()),
                new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "tourist")
            })
        );
        return new ControllerContext { HttpContext = ctx };
    }

    private ClubMessagesController CreateController(IServiceScope scope, long userId)
    {
        return new ClubMessagesController(
            scope.ServiceProvider.GetRequiredService<IClubMessageService>(),
            scope.ServiceProvider.GetRequiredService<INotificationService>(),
            scope.ServiceProvider.GetRequiredService<IHubContext<MessageHub>>(),
            scope.ServiceProvider.GetRequiredService<IUserRepository>()
        )
        {
            ControllerContext = FakeUser(userId)
        };
    }

    [Fact]
    public void Gets_all_messages_for_club()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        db.ClubMessages.Add(new ClubMessage(-1, -21, "Test msg 1"));
        db.ClubMessages.Add(new ClubMessage(-1, -21, "Test msg 2"));
        db.SaveChanges();

        var controller = CreateController(scope, -21);

        var result = controller.Get(-1).Result as OkObjectResult;
        result.ShouldNotBeNull();

        var list = result!.Value as List<ClubMessageDto>;
        list.ShouldNotBeNull();
        list!.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public void Creates_message_for_club()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        db.ClubMessages.RemoveRange(db.ClubMessages);
        db.SaveChanges();

        var controller = CreateController(scope, -21);

        var dto = new ClubMessageCreateDto
        {
            Text = "Hello club!",
            ResourceId = null,
            ResourceType = null
        };

        // Create vraća Task<ActionResult<ClubMessageDto>>
        var action = controller.Create(-1, dto).Result;

        // IActionResult je u action.Result
        var createdResult = action.Result as CreatedResult;
        createdResult.ShouldNotBeNull();
        createdResult!.StatusCode.ShouldBe(201);

        var createdDto = createdResult.Value as ClubMessageDto;
        createdDto.ShouldNotBeNull();
        createdDto!.Text.ShouldBe("Hello club!");
        createdDto.AuthorId.ShouldBe(-21);
        createdDto.ClubId.ShouldBe(-1);
    }

    [Fact]
    public void Updates_own_message()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var msg = new ClubMessage(-1, -21, "Old msg");
        db.ClubMessages.Add(msg);
        db.SaveChanges();

        var controller = CreateController(scope, -21);

        var dto = new ClubMessageCreateDto { Text = "Edited message" };

        var result = controller.Update(-1, msg.Id, dto).Result as OkObjectResult;
        result.ShouldNotBeNull();

        var updated = result!.Value as ClubMessageDto;
        updated.ShouldNotBeNull();
        updated!.Text.ShouldBe("Edited message");
    }

    [Fact]
    public void Fails_to_update_others_message()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var msg = new ClubMessage(-1, -21, "Not yours!");
        db.ClubMessages.Add(msg);
        db.SaveChanges();

        var controller = CreateController(scope, -22);

        var dto = new ClubMessageCreateDto { Text = "Hacking attempt" };

        Should.Throw<ForbiddenException>(() =>
        {
            controller.Update(-1, msg.Id, dto);
        });
    }

    [Fact]
    public void Deletes_own_message()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var msg = new ClubMessage(-1, -21, "To delete");
        db.ClubMessages.Add(msg);
        db.SaveChanges();

        var controller = CreateController(scope, -21);

        var result = controller.Delete(-1, msg.Id) as NoContentResult;
        result.ShouldNotBeNull();
        result!.StatusCode.ShouldBe(204);
    }

    [Fact]
    public void Fails_to_delete_others_message()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var msg = new ClubMessage(-1, -21, "Not yours");
        db.ClubMessages.Add(msg);
        db.SaveChanges();

        var controller = CreateController(scope, -22);

        Should.Throw<ForbiddenException>(() =>
        {
            controller.Delete(-1, msg.Id);
        });
    }
}
