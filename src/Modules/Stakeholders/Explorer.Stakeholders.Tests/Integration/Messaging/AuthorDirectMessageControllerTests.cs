using Explorer.API.Controllers.Author;
using Explorer.API.Hubs;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace Explorer.Stakeholders.Tests.Integration.Messaging
{
    [Collection("Sequential")]
    public class AuthorDirectMessageControllerTests : BaseStakeholdersIntegrationTest
    {
        public AuthorDirectMessageControllerTests(StakeholdersTestFactory factory) : base(factory) { }

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
                Content = "Author message",
                ResourceUrl = null
            };

            var sendAction = controller.SendMessage(dto);
            var sendOk = sendAction.Result.Result as OkObjectResult;
            sendOk.ShouldNotBeNull();
            var sent = sendOk!.Value as DirectMessageDto;
            sent.ShouldNotBeNull();

            sent!.Content = "Author message updated";
            var updateAction = controller.UpdateMessage(sent);
            var updateOk = updateAction.Result as OkObjectResult;
            updateOk.ShouldNotBeNull();
            var updated = updateOk!.Value as DirectMessageDto;
            updated.ShouldNotBeNull();
            updated!.Content.ShouldBe("Author message updated");

            var deleteResult = controller.DeleteMessage(sent.Id) as OkResult;
            deleteResult.ShouldNotBeNull();
        }

        private static AuthorDirectMessageController CreateController(IServiceScope scope, string userId)
        {
            var controller = new AuthorDirectMessageController(
                scope.ServiceProvider.GetRequiredService<IDirectMessageService>(),
                scope.ServiceProvider.GetRequiredService<INotificationService>(),
                scope.ServiceProvider.GetRequiredService<IHubContext<MessageHub>>(),
                scope.ServiceProvider.GetRequiredService<IUserRepository>());

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
