using System.Collections.Generic;
using System.Linq;
using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Clubs;

[Collection("Sequential")]
public class ClubControllerTests : BaseStakeholdersIntegrationTest
{
    public ClubControllerTests(StakeholdersTestFactory factory) : base(factory) { }

    private static ClubController CreateController(IServiceScope scope, string userId)
    {
        return new ClubController(
            scope.ServiceProvider.GetRequiredService<IClubService>()
        )
        {
            ControllerContext = BuildContext(userId)
        };
    }

    
    [Fact]
    public void Gets_all_seeded_clubs()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-21");

        // Act
        ActionResult<List<ClubDto>> actionResult = controller.GetAll();
        var okResult = actionResult.Result as OkObjectResult;

        // Assert
        okResult.ShouldNotBeNull();

        var clubs = okResult!.Value as List<ClubDto>;
        clubs.ShouldNotBeNull();
            clubs!.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

        [Fact]
        public void GetMine_returns_owned_clubs()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-21");

            var actionResult = controller.GetMine();
            var okResult = actionResult.Result as OkObjectResult;

            okResult.ShouldNotBeNull();
            var clubs = okResult!.Value as List<ClubDto>;
            clubs.ShouldNotBeNull();
            clubs!.Any(c => c.Id == -1).ShouldBeTrue();
            clubs.Any(c => c.Id == -2).ShouldBeTrue();
        }

        [Fact]
        public void Create_update_and_delete_club()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-21");
            var service = scope.ServiceProvider.GetRequiredService<IClubService>();

            var createDto = new ClubDto
            {
                Name = "Test Club",
                Description = "Test description",
                ImageUrls = new List<string> { "test" }
            };

            var createAction = controller.Create(createDto);
            var created = (createAction.Result as CreatedResult)!.Value as ClubDto;
            created.ShouldNotBeNull();

            created!.Name = "Updated Club";
            var updateAction = controller.Update(created.Id, created);
            var updateOk = updateAction.Result as OkObjectResult;
            updateOk.ShouldNotBeNull();
            var updated = updateOk!.Value as ClubDto;
            updated.ShouldNotBeNull();
            updated!.Name.ShouldBe("Updated Club");

            var deleteResult = controller.Delete(created.Id) as NoContentResult;
            deleteResult.ShouldNotBeNull();

            var all = service.GetAll();
            all.Any(c => c.Id == created.Id).ShouldBeFalse();
        }

        [Fact]
        public void Invite_accept_and_remove_member()
        {
            using var scope = Factory.Services.CreateScope();
            var ownerController = CreateController(scope, "-21");
            var memberController = CreateController(scope, "-22");
            var service = scope.ServiceProvider.GetRequiredService<IClubService>();

            var createDto = new ClubDto
            {
                Name = "Invite Club",
                Description = "Invite flow",
                ImageUrls = new List<string> { "test" }
            };

            var created = (ownerController.Create(createDto).Result as CreatedResult)!.Value as ClubDto;
            created.ShouldNotBeNull();

            var inviteResult = ownerController.Invite(created!.Id, -22) as OkResult;
            inviteResult.ShouldNotBeNull();

            var acceptResult = memberController.AcceptInvite(created.Id) as OkResult;
            acceptResult.ShouldNotBeNull();

            var club = service.GetAll().First(c => c.Id == created.Id);
            club.Members.ShouldContain(-22);

            var removeResult = ownerController.RemoveMember(created.Id, -22) as OkResult;
            removeResult.ShouldNotBeNull();

            club = service.GetAll().First(c => c.Id == created.Id);
            club.Members.ShouldNotContain(-22);

            ownerController.Delete(created.Id);
        }

        [Fact]
        public void Request_join_accept_and_decline()
        {
            using var scope = Factory.Services.CreateScope();
            var ownerController = CreateController(scope, "-21");
            var joinerController = CreateController(scope, "-23");
            var service = scope.ServiceProvider.GetRequiredService<IClubService>();

            var createDto = new ClubDto
            {
                Name = "Join Club",
                Description = "Join flow",
                ImageUrls = new List<string> { "test" }
            };

            var created = (ownerController.Create(createDto).Result as CreatedResult)!.Value as ClubDto;
            created.ShouldNotBeNull();

            var requestResult = joinerController.RequestToJoinClub(created!.Id) as OkResult;
            requestResult.ShouldNotBeNull();

            var acceptResult = ownerController.AcceptJoinRequest(created.Id, -23) as OkResult;
            acceptResult.ShouldNotBeNull();

            var club = service.GetAll().First(c => c.Id == created.Id);
            club.Members.ShouldContain(-23);

            var createDto2 = new ClubDto
            {
                Name = "Decline Club",
                Description = "Decline flow",
                ImageUrls = new List<string> { "test" }
            };

            var created2 = (ownerController.Create(createDto2).Result as CreatedResult)!.Value as ClubDto;
            created2.ShouldNotBeNull();

            var requestResult2 = joinerController.RequestToJoinClub(created2!.Id) as OkResult;
            requestResult2.ShouldNotBeNull();

            var declineResult = ownerController.DeclineJoinRequest(created2.Id, -23) as OkResult;
            declineResult.ShouldNotBeNull();

            var club2 = service.GetAll().First(c => c.Id == created2.Id);
            club2.Members.ShouldNotContain(-23);

            ownerController.Delete(created.Id);
            ownerController.Delete(created2.Id);
        }

        [Fact]
        public void Invite_by_username_adds_invite()
        {
            using var scope = Factory.Services.CreateScope();
            var ownerController = CreateController(scope, "-21");
            var service = scope.ServiceProvider.GetRequiredService<IClubService>();

            var createDto = new ClubDto
            {
                Name = "Username Club",
                Description = "Invite username",
                ImageUrls = new List<string> { "test" }
            };

            var created = (ownerController.Create(createDto).Result as CreatedResult)!.Value as ClubDto;
            created.ShouldNotBeNull();

            var inviteResult = ownerController.InviteByUsername(created!.Id, "turista2@gmail.com") as OkResult;
            inviteResult.ShouldNotBeNull();

            var club = service.GetAll().First(c => c.Id == created.Id);
            club.InvitedTourist.ShouldContain(-22);

            ownerController.Delete(created.Id);
        }


}
