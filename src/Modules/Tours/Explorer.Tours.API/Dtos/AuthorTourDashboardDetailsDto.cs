using System.Collections.Generic;

namespace Explorer.Tours.API.Dtos;

public class AuthorTourDashboardDetailsDto
{
    public TourDto Tour { get; set; } = new();

    public double Popularity { get; set; }
    public int RatingsCount { get; set; }

    public int Starts { get; set; }
    public int Completed { get; set; }
    public int Abandoned { get; set; }
    public int Active { get; set; }

    public List<TrendPointDto> StartsTrend { get; set; } = new();
    public List<TrendPointDto> CompletedTrend { get; set; } = new();
    public List<TrendPointDto> AbandonedTrend { get; set; } = new();

    public List<TourReviewDTO> LatestReviews { get; set; } = new();
}
