using System;
using System.Collections.Generic;

namespace Explorer.Tours.API.Dtos;

public class TourExecutionDto
{
    public long Id { get; set; }
    public long TouristId { get; set; }
    public int TourId { get; set; }
    public DateTime StartTime { get; set; }
    public double StartLatitude { get; set; }
    public double StartLongitude { get; set; }
    public TourExecutionStatusDto Status { get; set; }
    public DateTime? EndTime { get; set; }
    public TourPointDto? NextKeyPoint { get; set; }
    public List<CompletedTourPointDto> CompletedPoints { get; set; } = new();
}

public enum TourExecutionStatusDto
{
    Active = 0,
    Completed = 1,
    Abandoned = 2
}

