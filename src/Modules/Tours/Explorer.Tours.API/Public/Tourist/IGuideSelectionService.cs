using System.Collections.Generic;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface IGuideSelectionService
{
    IEnumerable<GuideDto> GetAvailableGuides(int tourId, long executionId, long touristId);
    void SelectGuide(long executionId, long touristId, long guideId);
    void CancelGuide(long executionId, long touristId);
    GuideDto? GetSelectedGuide(long executionId, long touristId);
}
