using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public.Tourist
{
    public interface ITourReviewService
    {
        TourReviewResponseDto CreateReview(int tourId, long touristId, TourReviewCreateDto reviewDto);
        TourReviewResponseDto UpdateReview(int tourId, long touristId, TourReviewCreateDto reviewDto);
        ReviewEligibilityDto CheckReviewEligibility(int tourId, long touristId);
        TourReviewResponseDto GetReview(int tourId, long touristId);
    }
}