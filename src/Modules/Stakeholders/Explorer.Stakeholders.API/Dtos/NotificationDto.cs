using System;

namespace Explorer.Stakeholders.API.Dtos
{
    public class NotificationDto
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ResourceUrl { get; set; }
        public string Type { get; set; }
        public long? ActorId { get; set; }
        public string? ActorUsername { get; set; }
        public int Count { get; set; }
        public long? ClubId { get; set; }

    }
}
