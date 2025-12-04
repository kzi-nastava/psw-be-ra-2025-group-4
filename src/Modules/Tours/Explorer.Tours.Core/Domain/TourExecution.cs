using System;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Tours.Core.Domain;

public class CompletedTourPoint
{
    public int Id { get; private set; }
    public int TourPointId { get; private set; }
    public DateTime CompletedAt { get; private set; }

    private CompletedTourPoint() { }

    public CompletedTourPoint(int tourPointId)
    {
        TourPointId = tourPointId;
        CompletedAt = DateTime.UtcNow;
    }
}

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
    public DateTime LastActivity { get; private set; }
    public ICollection<CompletedTourPoint> CompletedPoints { get; private set; }
    public double CompletionPercentage { get; private set; }


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
        CompletedPoints = new List<CompletedTourPoint>();
        LastActivity = StartTime;
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

    public void RegisterActivity()
    {
        LastActivity = DateTime.UtcNow;
    }

    public bool TryCompletePoint(int tourPointId)
    {
        if (Status != TourExecutionStatus.Active)
            throw new InvalidOperationException("Cannot complete point on inactive tour execution."); 

        if (CompletedPoints.Any(p => p.TourPointId == tourPointId))
            return false;

        CompletedPoints.Add(new CompletedTourPoint(tourPointId));
        RegisterActivity();
        return true;
    }

    public void UpdateCompletionPercentage(int totalTourPoints)
    {
        if (totalTourPoints <= 0)
        {
            CompletionPercentage = 0;
            return;
        }

        CompletionPercentage = (CompletedPoints.Count / (double)totalTourPoints) * 100;
    }

    public bool CanLeaveReview()
    {
        if (CompletionPercentage <= 35)
            return false;

        var daysSinceLastActivity = (DateTime.UtcNow - LastActivity).TotalDays;
        if (daysSinceLastActivity > 7)
            return false;

        return true;
    }

    public string GetReviewIneligibilityReason()
    {
        if (CompletionPercentage <= 35)
        {
            return "You have to complete at least 35% of the tour to leave a review.";
        }

        var daysSinceLastActivity = (DateTime.UtcNow - LastActivity).TotalDays;
        if (daysSinceLastActivity > 7)
        {
            return "More than 7 days have passed since your last activity. You cannot leave a review.";
        }

        return string.Empty;
    }
}

