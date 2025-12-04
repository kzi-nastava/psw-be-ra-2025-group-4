using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface ITourExecutionService
{
    TourExecutionDto StartTour(TourExecutionCreateDto dto, long touristId);
    TourExecutionDto GetById(long executionId, long touristId);
    TourExecutionDto Complete(long executionId, long touristId);
    TourExecutionDto Abandon(long executionId, long touristId);
    TourExecutionDto? GetActiveByTour(int tourId, long touristId);
}

