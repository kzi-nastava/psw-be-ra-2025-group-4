using Explorer.API.Controllers.Message;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Linq;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration.Notifications
{
    [Collection("Sequential")]
    public class MessageNotificationTests : BaseStakeholdersIntegrationTest
    {
        public MessageNotificationTests(StakeholdersTestFactory factory) : base(factory) { }

        private static NotificationController CreateController(IServiceScope scope, string personId)
        {
            return new NotificationController(
                scope.ServiceProvider.GetRequiredService<INotificationService>())
            {
                ControllerContext = BuildContext(personId)
            };
        }

        [Fact]
        public void GetPaged_returns_notifications_for_user()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-12"); // Autor2

            var result = controller.GetPaged(0, 10).Result as OkObjectResult;
            result.ShouldNotBeNull();

            var paged = result.Value as PagedResult<NotificationDto>;
            paged.ShouldNotBeNull();
        }

        [Fact]
        public void MarkAsRead_marks_single_notification()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-12");

            var paged = (controller.GetPaged(0, 10).Result as OkObjectResult)!
                .Value as PagedResult<NotificationDto>;

            var notif = paged!.Results.First();

            var response = controller.MarkAsRead(notif.Id);
            response.ShouldBeOfType<OkResult>();

            var refreshed = (controller.GetPaged(0, 10).Result as OkObjectResult)!
                .Value as PagedResult<NotificationDto>;

            refreshed!.Results
                .First(n => n.Id == notif.Id)
                .IsRead.ShouldBeTrue();
        }

        [Fact]
        public void MarkAll_marks_all_notifications_as_read()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-12");

            controller.MarkAll();

            var paged = (controller.GetPaged(0, 10).Result as OkObjectResult)!
                .Value as PagedResult<NotificationDto>;

            paged!.Results.All(n => n.IsRead).ShouldBeTrue();
        }
    }
}
