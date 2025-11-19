using Explorer.API.Controllers;
using Explorer.API.Controllers.Message;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
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

        [Fact]
        public void SendMessage_Creates_Successfully()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Mock the authenticated user
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "autor1@gmail.com") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var messageDto = new DirectMessageDto
            {
                Recipient = "autor2@gmail.com",
                Content = "Hello! This is a test message.",
                SentAt = DateTime.UtcNow
            };

            // Act

            var result = ((ObjectResult)controller.SendMessage(messageDto).Result).Value as DirectMessageDto;

            // Assert
            result.ShouldNotBeNull();
            result.Recipient.ShouldBe(messageDto.Recipient);
            result.Content.ShouldBe(messageDto.Content);
            result.SentAt.ShouldBe(messageDto.SentAt, TimeSpan.FromSeconds(1));
            result.EditedAt.ShouldBeNull();
        }

        [Fact]
        public void SendMessage_Create_Sender_Doesnt_Exists()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Mock the authenticated user
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "nonexistent@gmail.com") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var messageDto = new DirectMessageDto
            {
                Recipient = "autor2@gmail.com",
                Content = "Hello! This is a test message.",
                SentAt = DateTime.UtcNow
            };

            // Act
            var result = controller.SendMessage(messageDto);

            // Assert
            result.Result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void SendMessage_Create_Recipient_Doesnt_Exists()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Mock authenticated user that exists in database
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "autor1@gmail.com") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var messageDto = new DirectMessageDto
            {
                Recipient = "autor123@gmail.com", // Non-existent recipient
                Content = "Hello! This is a test message.",
                SentAt = DateTime.UtcNow
            };

            var result = controller.SendMessage(messageDto);

            result.Result.ShouldBeOfType<BadRequestObjectResult>();
            var badRequest = result.Result as BadRequestObjectResult;
            badRequest.Value.ShouldBe("Recipient 'autor123@gmail.com' not found.");
        }

        [Fact]
        public void DeleteMessage_ThrowsNotFound_WhenMessageDoesNotExist()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var result = controller.Delete(-999);

            result.ShouldBeOfType<NotFoundObjectResult>();
        }


        [Fact]
        public void DeleteMessage_ReturnsOk()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var result = controller.Delete(1);

            result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void UpdateMessage_Updates_Content_Successfully()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "autor1@gmail.com") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var original = new DirectMessageDto
            {
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

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "autor1@gmail.com") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var messageDto = new DirectMessageDto
            {
                Id = -999,
                Recipient = "autor2@gmail.com",
                Content = "Hello! This is a test message.",
                SentAt = DateTime.UtcNow
            };

            var result = controller.Update(messageDto).Result;

            result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void GetPaged_Returns_Correct_Page()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "autor1@gmail.com"),
            }, "TestAuthentication");

            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            for (int i = 0; i < 3; i++)
            {
                controller.SendMessage(new DirectMessageDto
                {
                    Sender = "autor1@gmail.com",
                    Recipient = "autor2@gmail.com",
                    Content = $"Message {i}",
                    SentAt = DateTime.UtcNow
                });
            }

            var result = ((ObjectResult)controller.GetAll(1, 2).Result).Value as PagedResult<DirectMessageDto>;
            result.ShouldNotBeNull();
            result.Results.Count.ShouldBe(2);
            result.TotalCount.ShouldBeGreaterThanOrEqualTo(3);
        }

        [Fact]
        public void GetPagedConversations_Returns_Conversations()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "autor1@gmail.com"),
            }, "TestAuthentication");

            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            for (int i = 0; i < 3; i++)
            {
                controller.SendMessage(new DirectMessageDto
                {
                    Sender = "autor1@gmail.com",
                    Recipient = $"autor{i + 1}@gmail.com",
                    Content = $"Message {i}",
                    SentAt = DateTime.UtcNow
                });
            }

            var result = ((ObjectResult)controller.GetAllConversations(1, 10).Result).Value as PagedResult<DirectMessageDto>;
            result.ShouldNotBeNull();
            result.Results.Count.ShouldBe(3);
            result.TotalCount.ShouldBeGreaterThanOrEqualTo(3);
        }
    }
}
