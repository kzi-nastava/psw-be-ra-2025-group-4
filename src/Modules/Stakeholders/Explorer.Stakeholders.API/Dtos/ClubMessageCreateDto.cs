namespace Explorer.Stakeholders.API.Dtos
{
    public class ClubMessageCreateDto
    {
        public string Text { get; set; }
        public long? ResourceId { get; set; }
        public string? ResourceType { get; set; }
    }
}
