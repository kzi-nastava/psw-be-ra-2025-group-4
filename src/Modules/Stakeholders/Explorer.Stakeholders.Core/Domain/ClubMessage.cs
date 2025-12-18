using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class ClubMessage : Entity
    {
        public long ClubId { get; private set; }
        public long AuthorId { get; private set; } 
        public string Text { get; private set; }
        public long? ResourceId { get; private set; }
        public string? ResourceType { get; private set; } 
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private ClubMessage() { } 

        public ClubMessage(long clubId, long authorId, string text, long? resourceId = null, string? resourceType = null)
        {
            ClubId = clubId;
            AuthorId = authorId;
            SetText(text);

            ResourceId = resourceId;
            ResourceType = resourceType;

            CreatedAt = DateTime.UtcNow;
        }

        public void SetText(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length > 280)
                throw new ArgumentException("Poruka mora imati 1-280 karaktera.");

            Text = text;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
