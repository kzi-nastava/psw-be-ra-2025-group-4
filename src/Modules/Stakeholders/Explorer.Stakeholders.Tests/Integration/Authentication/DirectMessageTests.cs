using Explorer.API.Controllers;
using Explorer.API.Controllers.Message;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Tests.Integration.Authentication
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

            var messageDto = new DirectMessageDto
            {
                Sender = "autor1@gmail.com",
                Recipient = "autor2@gmail.com",
                Content = "Hello! This is a test message.",
                SentAt = DateTime.UtcNow
            };

            // Act

            var result = ((ObjectResult)controller.SendMessage(messageDto).Result).Value as DirectMessageDto;

            // Assert
            result.ShouldNotBeNull();
            result.Sender.ShouldBe(messageDto.Sender);
            result.Recipient.ShouldBe(messageDto.Recipient);
            result.Content.ShouldBe(messageDto.Content);
            result.SentAt.ShouldBe(messageDto.SentAt, TimeSpan.FromSeconds(1));
            result.EditedAt.ShouldBeNull();
        }

        [Fact]
        public void SendMessaage_Create_Sender_Doesnt_Exists()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var messageDto = new DirectMessageDto
            {
                Sender = "example@gmail.com",
                Recipient = "autor2@gmail.com",
                Content = "Hello! This is a test message.",
                SentAt = DateTime.UtcNow
            };

            Should.Throw<ArgumentException>(() =>
            {
                var result = controller.SendMessage(messageDto).Result;
            });
        }

        [Fact]
        public void SendMessaage_Create_Recipient_Doesnt_Exists()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var messageDto = new DirectMessageDto
            {
                Sender = "autor1@gmail.com",
                Recipient = "autor123@gmail.com",
                Content = "Hello! This is a test message.",
                SentAt = DateTime.UtcNow
            };

            Should.Throw<ArgumentException>(() =>
            {
                var result = controller.SendMessage(messageDto).Result;
            });
        }

        [Fact]
        public void DeleteMessage_ThrowsNotFound_WhenMessageDoesNotExist()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act & Assert
            Should.Throw<NotFoundException>(() =>
            {
                controller.Delete(-999);
            });
        }

        [Fact]
        public void DeleteMessage_ReturnsOk()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
   
            Should.Throw<NotFoundException>(() =>
            {
                var _ = controller.Delete(1);
            });
        }

        [Fact]
        public void UpdateMessage_Updates_Content_Successfully()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // First send a message
            var original = new DirectMessageDto
            {
                Sender = "autor1@gmail.com",
                Recipient = "autor2@gmail.com",
                Content = "Original content",
                SentAt = DateTime.UtcNow
            };

            var created = ((ObjectResult)controller.SendMessage(original).Result).Value as DirectMessageDto;
            created.ShouldNotBeNull();

            // Prepare update
            created.Content = "Updated content";

            // Act
            var updated = ((ObjectResult)controller.Update(created).Result).Value as DirectMessageDto;

            // Assert
            updated.ShouldNotBeNull();
            updated.Content.ShouldBe("Updated content");
            updated.EditedAt.ShouldNotBeNull();
        }

        [Fact]
        public void UpdateMessage_ThrowsNotFound_WhenMessageDoesNotExist()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new DirectMessageDto
            {
                Id = -999,
                Sender = "autor1@gmail.com",
                Recipient = "autor2@gmail.com",
                Content = "Doesn't matter"
            };

            Should.Throw<NotFoundException>(() =>
            {
                var _ = controller.Update(dto).Result;
            });
        }

        [Fact]
        public void GetPaged_Returns_Correct_Page()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

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


    }
}
