namespace Explorer.Tours.API.Dtos;

public class TourPointSecretDto
{
    public long TourPointId { get; set; }
    public string? Secret { get; set; }
    public bool IsUnlocked { get; set; }
}


