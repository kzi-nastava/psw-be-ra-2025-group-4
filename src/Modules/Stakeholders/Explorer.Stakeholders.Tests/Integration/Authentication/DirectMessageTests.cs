using Explorer.API.Controllers;
using Explorer.API.Controllers.Message;
using Explorer.BuildingBlocks.Core.Exceptions;
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

            // Act - try to delete a message that does NOT exist
            var result = controller.Delete(1);

            // Assert
            result.ShouldBeOfType<OkResult>();
        }

    }
}
