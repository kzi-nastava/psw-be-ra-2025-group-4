using System;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Tours.Core.Domain;

public enum TourExecutionStatus
{
    Active = 0,
    Completed = 1,
    Abandoned = 2
}

public class TourExecution : AggregateRoot
{
    private TourExecution() { }

    public long TouristId { get; private set; }
    public int TourId { get; private set; }
    public DateTime StartTime { get; private set; }
    public double StartLatitude { get; private set; }
    public double StartLongitude { get; private set; }
    public TourExecutionStatus Status { get; private set; }
    public DateTime? EndTime { get; private set; }

    public TourExecution(long touristId, int tourId, double startLatitude, double startLongitude)
    {
        if (touristId == 0)
            throw new ArgumentException("Tourist ID must be valid.", nameof(touristId));
        if (tourId == 0)
            throw new ArgumentException("Tour ID must be valid.", nameof(tourId));
        if (startLatitude < -90 || startLatitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90.", nameof(startLatitude));
        if (startLongitude < -180 || startLongitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180.", nameof(startLongitude));

        TouristId = touristId;
        TourId = tourId;
        StartTime = DateTime.UtcNow;
        StartLatitude = startLatitude;
        StartLongitude = startLongitude;
        Status = TourExecutionStatus.Active;
    }

    public void Complete()
    {
        if (Status != TourExecutionStatus.Active)
            throw new InvalidOperationException("Only active tour executions can be completed.");

        Status = TourExecutionStatus.Completed;
        EndTime = DateTime.UtcNow;
    }

    public void Abandon()
    {
        if (Status != TourExecutionStatus.Active)
            throw new InvalidOperationException("Only active tour executions can be abandoned.");

        Status = TourExecutionStatus.Abandoned;
        EndTime = DateTime.UtcNow;
    }
}

