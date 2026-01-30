namespace Explorer.Tours.Core.Domain;

public class GuideTour
{
    public long GuideId { get; private set; }
    public int TourId { get; private set; }
    public Guide Guide { get; private set; } = default!;

    private GuideTour() { }

    public GuideTour(long guideId, int tourId)
    {
        GuideId = guideId;
        TourId = tourId;
    }
}
