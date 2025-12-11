using System;

namespace Explorer.Tours.API.Dtos;

public class CompletedTourPointDto
{
    public int Id { get; set; }
    public int TourPointId { get; set; }
    public DateTime CompletedAt { get; set; }
    public int? Order { get; set; }
    public string? Name { get; set; }
}

