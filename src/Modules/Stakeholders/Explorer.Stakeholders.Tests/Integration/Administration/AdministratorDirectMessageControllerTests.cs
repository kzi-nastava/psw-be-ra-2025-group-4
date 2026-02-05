using System.Security.Claims;
using Explorer.API.Controllers.Administrator;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Administration
{
    [Collection("Sequential")]
    public class AdministratorDirectMessageControllerTests : BaseStakeholdersIntegrationTest
    {
        public AdministratorDirectMessageControllerTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void GetConversations_returns_paged_conversations()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");

            var action = controller.GetConversations(1, 10);
            var result = action.Result as OkObjectResult;
            result.ShouldNotBeNull();
            var page = result!.Value as PagedResult<DirectMessageDto>;
            page.ShouldNotBeNull();
            page.TotalCount.ShouldBe(1);
        }

        [Fact]
        public void GetMessageHistory_returns_messages()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");

            var action = controller.GetMessageHistory(-12, 1, 10);
            var result = action.Result as OkObjectResult;
            result.ShouldNotBeNull();
            var page = result!.Value as PagedResult<DirectMessageDto>;
            page.ShouldNotBeNull();
            page.TotalCount.ShouldBeGreaterThanOrEqualTo(3);
        }

        [Fact]
        public void StartEmptyConversation_returns_user_id()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");

            var action = controller.StartEmptyConversation(new StartConversationDto { Username = "autor2@gmail.com" });
            var result = action.Result as OkObjectResult;
            result.ShouldNotBeNull();
            result!.Value.ShouldBe(-12L);
        }

        [Fact]
        public void Send_update_and_delete_message()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");

            var dto = new DirectMessageDto
            {
                RecipientId = -12,
                Recipient = "autor2@gmail.com",
                Content = "Admin message",
                ResourceUrl = null
            };

            var sendAction = controller.SendMessage(dto);
            var sendOk = sendAction.Result as OkObjectResult;
            sendOk.ShouldNotBeNull();
            var sent = sendOk!.Value as DirectMessageDto;
            sent.ShouldNotBeNull();

            sent!.Content = "Admin message updated";
            var updateAction = controller.UpdateMessage(sent);
            var updateOk = updateAction.Result as OkObjectResult;
            updateOk.ShouldNotBeNull();
            var updated = updateOk!.Value as DirectMessageDto;
            updated.ShouldNotBeNull();
            updated!.Content.ShouldBe("Admin message updated");

            var deleteResult = controller.DeleteMessage(sent.Id) as OkResult;
            deleteResult.ShouldNotBeNull();
        }

        private static AdministratorDirectMessageController CreateController(IServiceScope scope, string userId)
        {
            var controller = new AdministratorDirectMessageController(
                scope.ServiceProvider.GetRequiredService<IDirectMessageService>());

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("id", userId)
            }, "TestAuth"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            return controller;
        }
    }
}
