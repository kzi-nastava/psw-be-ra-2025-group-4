using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration.Messaging
{
    [Collection("Sequential")]
    public class TouristDirectMessageCrudTests : BaseStakeholdersIntegrationTest
    {
        public TouristDirectMessageCrudTests(StakeholdersTestFactory factory) : base(factory) { }

        private static TouristDirectMessageController CreateController(IServiceScope scope, string personId)
        {
            return new TouristDirectMessageController(scope.ServiceProvider.GetRequiredService<IDirectMessageService>())
            {
                ControllerContext = BaseStakeholdersIntegrationTest.BuildContext(personId)
            };
        }

        [Fact]
        public void GetConversations_ReturnsPagedResult()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-23");

            var seedDto = new DirectMessageDto
            {
                RecipientId = -22,
                Recipient = "turista2@gmail.com",
                Content = "Hello for conversations test",
                SentAt = DateTime.UtcNow,
                ResourceUrl = null
            };
            controller.SendMessage(seedDto);

            var result = controller.GetConversations().Result as OkObjectResult;
            result.ShouldNotBeNull();

            var paged = result.Value as PagedResult<DirectMessageDto>;
            paged.ShouldNotBeNull();
            paged.Results.ShouldNotBeEmpty();

            paged.Results.Any(m => m.Content == "Hello for conversations test").ShouldBeTrue();
        }


        [Fact]
        public void GetMessageHistory_ReturnsMessagesBetweenUsers()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-23");

            var seedDto = new DirectMessageDto
            {
                RecipientId = -22,
                Recipient = "turista2@gmail.com",
                Content = "Hello for history test",
                SentAt = DateTime.UtcNow,
                ResourceUrl = null
            };

            var ret = controller.SendMessage(seedDto);

            Assert.IsType<ActionResult<DirectMessageDto>>(ret);

            var ok = ret.Result as OkObjectResult;
            ok.ShouldNotBeNull();

            var t = ok.Value as DirectMessageDto;
            t.ShouldNotBeNull();
            t.Content.ShouldBe("Hello for history test");

            var result = controller.GetMessageHistory(-22).Result as OkObjectResult;
            result.ShouldNotBeNull();

            var paged = result.Value as PagedResult<DirectMessageDto>;
            paged.ShouldNotBeNull();
            paged.Results.ShouldNotBeEmpty();
            paged.Results.All(m => m.RecipientId == -22 || m.SenderId == -23).ShouldBeTrue();
        }



        [Fact]
        public void UpdateMessage_ReturnsUpdatedMessage()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-23");

            var dto = new DirectMessageDto
            {
                RecipientId = -22,
                Recipient = "turista2@gmail.com",
                Content = "Original message",
                SentAt = DateTime.UtcNow,
                ResourceUrl = null
            };
            var sendResult = controller.SendMessage(dto).Result as OkObjectResult;
            sendResult.ShouldNotBeNull();
            var sent = sendResult.Value as DirectMessageDto;

            sent.Content = "Updated message";
            var updateResult = controller.UpdateMessage(sent).Result as OkObjectResult;
            updateResult.ShouldNotBeNull();
            var updated = updateResult.Value as DirectMessageDto;

            updated.ShouldNotBeNull();
            updated.Content.ShouldBe("Updated message");
        }

        [Fact]
        public void DeleteMessage_RemovesMessageFromHistory()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-23");

            var dto = new DirectMessageDto
            {
                RecipientId = -22,
                Recipient = "turista2@gmail.com",
                Content = "Message to delete",
                SentAt = DateTime.UtcNow,
                ResourceUrl = null
            };
            var sendResult = controller.SendMessage(dto).Result as OkObjectResult;
            sendResult.ShouldNotBeNull();
            var sent = sendResult.Value as DirectMessageDto;

            var deleteResult = controller.DeleteMessage(sent.Id) as OkResult;
            deleteResult.ShouldNotBeNull();

            var historyResult = controller.GetMessageHistory(-22).Result as OkObjectResult;
            historyResult.ShouldNotBeNull();
            var paged = historyResult.Value as PagedResult<DirectMessageDto>;
            paged.ShouldNotBeNull();
            paged.Results.All(m => m.Id != sent.Id).ShouldBeTrue();
        }

        [Fact]
        public void SendMessage_WithResource_AttachesCorrectResourceInfo()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-23");

            var dto = new DirectMessageDto
            {
                RecipientId = -22,
                Recipient = "turista2@gmail.com",
                Content = "Check this tour!",
                SentAt = DateTime.UtcNow,
                ResourceUrl = "/tours/103"
            };

            var result = controller.SendMessage(dto).Result as OkObjectResult;
            result.ShouldNotBeNull();

            var message = result.Value as DirectMessageDto;
            message.ShouldNotBeNull();
            message.Content.ShouldBe("Check this tour!");
            message.ResourceUrl.ShouldBe("/tours/103");
        }

    }
}
