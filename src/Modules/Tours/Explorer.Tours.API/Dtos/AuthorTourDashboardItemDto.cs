using System;

namespace Explorer.Tours.API.Dtos;

public class AuthorTourDashboardItemDto
{
    public int TourId { get; set; }
    public string Name { get; set; } = "";
    public TourDtoStatus Status { get; set; }

    public decimal Price { get; set; }
    public double LengthInKm { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }

    public double Popularity { get; set; }
    public int RatingsCount { get; set; }

    public int Starts { get; set; }
    public int Completed { get; set; }
    public int Abandoned { get; set; }
    public int Active { get; set; }
}
