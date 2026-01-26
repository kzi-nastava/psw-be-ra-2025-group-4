namespace Explorer.Stakeholders.API.Dtos
{
    public class TouristLookupDto
    {
        public long Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
