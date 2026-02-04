using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using System;

namespace Explorer.Stakeholders.Core.Domain
{
    public enum NotificationType
    {
        Message = 0,
        ClubActivity = 1,
        Follow = 2,
        ClubJoinRequestResponse = 3,
        AffiliateCodeAssigned = 4,
        AchievementUnlocked = 5
    }

    public class Notification : AggregateRoot
    {
        public long UserId { get; private set; }
        public string Content { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string? ResourceUrl { get; private set; }
        public NotificationType Type { get; private set; }
        public long? ActorId { get; private set; }
        public string? ActorUsername { get; private set; }
        public int Count { get; private set; } = 1;
        public long? ClubId { get; private set; }

        protected Notification() { }

        public Notification(long userId, string content, NotificationType type, string? resourceUrl = null, long ? actorId = null, string? actorUsername = null, long? clubId = null)
        {
            if (userId == 0)
                throw new EntityValidationException("Invalid user id.");

            if (string.IsNullOrWhiteSpace(content))
                throw new EntityValidationException("Notification content cannot be empty.");

            UserId = userId;
            Content = content;
            Type = type;
            ResourceUrl = resourceUrl;
            ActorId = actorId;
            ActorUsername = actorUsername;
            IsRead = false;
            CreatedAt = DateTime.UtcNow;
            ClubId = clubId;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
        public void Increment(string latestContent)
        {
            Count++;
            Content = latestContent;
            IsRead = false;
            CreatedAt = DateTime.UtcNow;
        }

    }
}
