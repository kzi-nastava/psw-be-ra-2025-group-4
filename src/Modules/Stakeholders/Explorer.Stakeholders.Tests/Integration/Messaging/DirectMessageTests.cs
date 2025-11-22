using Explorer.API.Controllers;
using Explorer.API.Controllers.Message;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Tests.Integration.Messaging
{
    [Collection("Sequential")]

    public class DirectMessageTests : BaseStakeholdersIntegrationTest
    {
        public DirectMessageTests(StakeholdersTestFactory factory) : base(factory) { }

        private static DirectMessageController CreateController(IServiceScope scope)
        {
            return new DirectMessageController(scope.ServiceProvider.GetRequiredService<IDirectMessageService>());
        }

        // ----------------------------------------------
        // Helper for setting user with personId
        // ----------------------------------------------
       /* private static void SetUser(ControllerBase controller, long personId)
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] { new Claim("personId", personId.ToString()) },
                    "mock"
                )
            );

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public void SendMessage_Returns_Created_Message()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            SetUser(controller, -11);  

            var dto = new DirectMessageDto
            {
                RecipientId = -12,
                Recipient = "autor2@gmail.com",
                Content = "Test message content",
                SentAt = DateTime.UtcNow
            };

            var result = controller.SendMessage(dto).Result as OkObjectResult;

            result.ShouldNotBeNull();
            var returned = result.Value as DirectMessageDto;

            returned.ShouldNotBeNull();
            returned.Content.ShouldBe("Test message content");
            returned.SentAt.ShouldNotBe(default);
        }

        [Fact]
        public void SendMessage_Create_Sender_Doesnt_Exists()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            SetUser(controller, -9999);   // nonexistent sender

            var dto = new DirectMessageDto
            {
                RecipientId = -12,
                Recipient = "autor2@gmail.com",
                Content = "Hello! This is a test message.",
                SentAt = DateTime.UtcNow
            };

            var result = controller.SendMessage(dto);

            result.Result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void SendMessage_Create_Recipient_Doesnt_Exists()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            SetUser(controller, -11);

            var dto = new DirectMessageDto
            {
                RecipientId = -999,
                Recipient = "autor2@gmail.com",
                Content = "Hello!",
                SentAt = DateTime.UtcNow
            };

            var result = controller.SendMessage(dto);

            result.Result.ShouldBeOfType<BadRequestObjectResult>();
            var bad = result.Result as BadRequestObjectResult;
            bad.Value.ShouldBe($"Recipient '{dto.RecipientId}' not found.");
        }

        [Fact]
        public void DeleteMessage_ThrowsNotFound_WhenMessageDoesNotExist()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            SetUser(controller, -11);

            var result = controller.Delete(-999);

            result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void DeleteMessage_ReturnsOk()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            SetUser(controller, -11);

            var result = controller.Delete(1);

            result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void UpdateMessage_Updates_Content_Successfully()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            SetUser(controller, -11);

            var original = new DirectMessageDto
            {
                RecipientId = -12,
                Recipient = "autor2@gmail.com",
                Content = "Original content",
                SentAt = DateTime.UtcNow
            };

            var created = ((ObjectResult)controller.SendMessage(original).Result).Value as DirectMessageDto;
            created.ShouldNotBeNull();

            created.Content = "Updated content";

            var updated = ((ObjectResult)controller.Update(created).Result).Value as DirectMessageDto;

            updated.ShouldNotBeNull();
            updated.Content.ShouldBe("Updated content");
            updated.EditedAt.ShouldNotBeNull();
        }

        [Fact]
        public void UpdateMessage_ThrowsNotFound_WhenMessageDoesNotExist()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            SetUser(controller, -11);

            var dto = new DirectMessageDto
            {
                Id = -999,
                Recipient = "autor2@gmail.com",
                Content = "Hello!",
                SentAt = DateTime.UtcNow
            };

            var result = controller.Update(dto).Result;

            result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void GetPaged_Returns_Correct_Page()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            SetUser(controller, -11);

            for (int i = 0; i < 3; i++)
            {
                controller.SendMessage(new DirectMessageDto
                {
                    RecipientId = -12,
                    Recipient = "autor2@gmail.com",
                    Content = $"Message {i}",
                    SentAt = DateTime.UtcNow
                });
            }

            var page = ((ObjectResult)controller.GetAll(1, 2).Result).Value as PagedResult<DirectMessageDto>;

            page.ShouldNotBeNull();
            page.Results.Count.ShouldBe(2);
            page.TotalCount.ShouldBeGreaterThanOrEqualTo(3);
        }

        [Fact]
        public void GetPagedConversations_Returns_Conversations()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            SetUser(controller, -11);

            for (int i = 0; i < 3; i++)
            {
                controller.SendMessage(new DirectMessageDto
                {
                    RecipientId = -12,
                    Recipient = "autor2@gmail.com",
                    Content = $"Message {i}",
                    SentAt = DateTime.UtcNow
                });
            }

            var result = ((ObjectResult)controller.GetAllConversations(1, 10).Result)
                .Value as PagedResult<DirectMessageDto>;

            result.ShouldNotBeNull();
            result.Results.Count.ShouldBe(3);
            result.Results[0].RecipientId.ShouldBe(-12);
            result.TotalCount.ShouldBeGreaterThanOrEqualTo(3);
        }*/
    }

}
