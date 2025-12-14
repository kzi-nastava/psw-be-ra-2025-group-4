using System;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration.Notifications
{
    public class NotificationTests
    {
        [Fact]
        public void Constructor_sets_defaults_and_properties()
        {
            // Arrange / Act
            var n = new Notification(
                userId: 10,
                content: "hello",
                type: NotificationType.Message,
                resourceUrl: "/tours/5",
                actorId: 101,
                actorUsername: "boris"
            );

            // Assert
            n.UserId.ShouldBe(10);
            n.Content.ShouldBe("hello");
            n.Type.ShouldBe(NotificationType.Message);
            n.ResourceUrl.ShouldBe("/tours/5");

            n.ActorId.ShouldBe(101);
            n.ActorUsername.ShouldBe("boris");

            n.IsRead.ShouldBeFalse();
            n.Count.ShouldBe(1);

            // CreatedAt je "sada" (ne proveravaj tačno, nego u intervalu)
            n.CreatedAt.ShouldBeLessThanOrEqualTo(DateTime.UtcNow.AddSeconds(1));
        }

        [Theory]
        [InlineData(0)]
        public void Constructor_throws_for_invalid_userId(long invalidUserId)
        {
            Should.Throw<EntityValidationException>(() =>
                new Notification(invalidUserId, "content", NotificationType.Message));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_throws_for_empty_content(string badContent)
        {
            Should.Throw<EntityValidationException>(() =>
                new Notification(1, badContent, NotificationType.Message));
        }

        [Fact]
        public void MarkAsRead_sets_IsRead_true()
        {
            // Arrange
            var n = new Notification(1, "hello", NotificationType.Message);
            n.IsRead.ShouldBeFalse();

            // Act
            n.MarkAsRead();

            // Assert
            n.IsRead.ShouldBeTrue();
        }

        [Fact]
        public void Increment_increases_count_and_updates_content_and_resets_read_and_updates_createdAt()
        {
            // Arrange
            var n = new Notification(
                userId: 1,
                content: "first",
                type: NotificationType.Message,
                actorId: 101,
                actorUsername: "boris"
            );

            n.MarkAsRead();
            n.IsRead.ShouldBeTrue();

            var oldCreatedAt = n.CreatedAt;
            var oldCount = n.Count;

            // malo “osiguranje” da CreatedAt stvarno promeni vrednost
            System.Threading.Thread.Sleep(10);

            // Act
            n.Increment("second");

            // Assert
            n.Count.ShouldBe(oldCount + 1);
            n.Content.ShouldBe("second");

            // Increment mora da vrati notif na unread (da bi se videlo kao nova)
            n.IsRead.ShouldBeFalse();

            // CreatedAt se refresuje
            n.CreatedAt.ShouldBeGreaterThan(oldCreatedAt);

            // Actor info se ne menja
            n.ActorId.ShouldBe(101);
            n.ActorUsername.ShouldBe("boris");
        }

        [Fact]
        public void Increment_can_be_called_multiple_times()
        {
            // Arrange
            var n = new Notification(1, "m1", NotificationType.Message, actorId: 50);

            // Act
            n.Increment("m2");
            n.Increment("m3");
            n.Increment("m4");

            // Assert
            n.Count.ShouldBe(4); // start 1 + 3 incrementa
            n.Content.ShouldBe("m4");
            n.IsRead.ShouldBeFalse();
        }

        // ============================
        // ClubMessageNotifications tests
        // ============================

        [Fact]
        public void Constructor_sets_club_activity_properties()
        {
            // Arrange / Act
            var n = new Notification(
                userId: 10,
                content: "New club message",
                type: NotificationType.ClubActivity,
                resourceUrl: null,
                actorId: 101,
                actorUsername: "boris",
                clubId: 555
            );

            // Assert
            n.UserId.ShouldBe(10);
            n.Type.ShouldBe(NotificationType.ClubActivity);
            n.Content.ShouldBe("New club message");

            n.ActorId.ShouldBe(101);
            n.ActorUsername.ShouldBe("boris");

            n.ClubId.ShouldBe(555);
            n.ResourceUrl.ShouldBeNull();

            n.IsRead.ShouldBeFalse();
            n.Count.ShouldBe(1);
        }

        [Fact]
        public void Club_activity_can_be_marked_as_read()
        {
            // Arrange
            var n = new Notification(
                userId: 1,
                content: "Club ping",
                type: NotificationType.ClubActivity,
                clubId: 101
            );

            n.IsRead.ShouldBeFalse();

            // Act
            n.MarkAsRead();

            // Assert
            n.IsRead.ShouldBeTrue();
        }

        [Fact]
        public void Increment_on_club_activity_increases_count_and_updates_content_and_keeps_clubId()
        {
            // Arrange
            var n = new Notification(
                userId: 1,
                content: "First msg",
                type: NotificationType.ClubActivity,
                actorId: 101,
                actorUsername: "boris",
                clubId: 999
            );

            var oldClubId = n.ClubId;
            var oldCreatedAt = n.CreatedAt;

            System.Threading.Thread.Sleep(10);

            // Act
            n.Increment("Second msg");

            // Assert
            n.Count.ShouldBe(2);
            n.Content.ShouldBe("Second msg");
            n.IsRead.ShouldBeFalse();
            n.CreatedAt.ShouldBeGreaterThan(oldCreatedAt);

            n.ClubId.ShouldBe(oldClubId);
        }
    }
}
