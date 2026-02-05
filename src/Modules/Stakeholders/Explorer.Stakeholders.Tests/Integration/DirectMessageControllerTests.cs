using Explorer.API.Controllers.Message;
using Explorer.API.Hubs;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration
{
    [Collection("Sequential")]
    public class DirectMessageControllerTests : BaseStakeholdersIntegrationTest
    {
        public DirectMessageControllerTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void GetAll_returns_paged_messages()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");

            var action = controller.GetAll(1, 10);
            var result = action.Result as OkObjectResult;
            result.ShouldNotBeNull();
            var page = result!.Value as PagedResult<DirectMessageDto>;
            page.ShouldNotBeNull();
            page.TotalCount.ShouldBeGreaterThanOrEqualTo(3);
        }

        [Fact]
        public void GetAllConversations_returns_paged_conversations()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");

            var action = controller.GetAllConversations(1, 10);
            var result = action.Result as OkObjectResult;
            result.ShouldNotBeNull();
            var page = result!.Value as PagedResult<DirectMessageDto>;
            page.ShouldNotBeNull();
            page.TotalCount.ShouldBe(1);
        }

        [Fact]
        public void GetAllBetweenUsers_returns_messages()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");

            var action = controller.GetAllBetweenUsers(-12, 1, 10);
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
        public void SearchUsers_returns_matching_users()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");

            var action = controller.SearchUsers("autor");
            var result = action.Result as OkObjectResult;
            result.ShouldNotBeNull();
            var users = result!.Value as IEnumerable<UserDto>;
            users.ShouldNotBeNull();
            users!.Count().ShouldBeGreaterThanOrEqualTo(3);
        }

        private static DirectMessageController CreateController(IServiceScope scope, string userId)
        {
            return new DirectMessageController(
                scope.ServiceProvider.GetRequiredService<IDirectMessageService>(),
                scope.ServiceProvider.GetRequiredService<IHubContext<MessageHub>>(),
                scope.ServiceProvider.GetRequiredService<INotificationService>(),
                scope.ServiceProvider.GetRequiredService<IUserRepository>())
            {
                ControllerContext = BuildContext(userId)
            };
        }
    }
}
