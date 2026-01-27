using System;

namespace Explorer.Tours.Core.Domain;

public enum GuideAssignmentStatus
{
    Active = 0,
    Cancelled = 1
}

public class GuideAssignment
{
    public long Id { get; private set; }
    public long GuideId { get; private set; }
    public long TourExecutionId { get; private set; }
    public GuideAssignmentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guide Guide { get; private set; } = default!;

    private GuideAssignment() { }

    public GuideAssignment(long guideId, long tourExecutionId)
    {
        GuideId = guideId;
        TourExecutionId = tourExecutionId;
        Status = GuideAssignmentStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status != GuideAssignmentStatus.Active)
            throw new InvalidOperationException("Only active assignments can be cancelled.");
        Status = GuideAssignmentStatus.Cancelled;
    }
}
