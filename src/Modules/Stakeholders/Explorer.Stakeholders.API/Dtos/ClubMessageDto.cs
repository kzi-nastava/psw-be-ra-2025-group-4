namespace Explorer.Stakeholders.API.Dtos
{
    public class ClubMessageDto
    {
        public long Id { get; set; }
        public long ClubId { get; set; }
        public long AuthorId { get; set; }
        public string Text { get; set; }
        public long? ResourceId { get; set; }
        public string? ResourceType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
