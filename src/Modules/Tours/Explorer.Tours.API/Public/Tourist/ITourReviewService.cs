using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public
{
    public interface ITourReviewService
    {
        TourReviewDTO Create(TourReviewDTO tourReviewDto);
        TourReviewDTO Update(TourReviewDTO tourReviewDto);
        void Delete(int id);
        TourReviewDTO GetById(int id);
        PagedResult<TourReviewDTO> GetPagedByTourist(int touristId, int page, int pageSize);
        PagedResult<TourReviewDTO> GetPagedByTour(int tourId, int page, int pageSize);
        TourReviewDTO GetByTouristAndTour(int touristId, int tourId);
        string GetTourAverageGrade(int tourId);
        ReviewEligibilityInfo GetReviewEligibilityInfo(int touristId, int tourId);
    }
}