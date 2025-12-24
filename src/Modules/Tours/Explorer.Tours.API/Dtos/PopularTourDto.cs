namespace Explorer.Tours.API.Dtos;

public class PopularTourDto
{
    public long TourId { get; set; }
    public string Name { get; set; } = "";
    public TourDtoStatus Status { get; set; }


    public string? Area { get; set; }

  
    public double PopularityScore { get; set; }
    public double AverageGrade { get; set; }
    public int RatingsCount { get; set; }
}
