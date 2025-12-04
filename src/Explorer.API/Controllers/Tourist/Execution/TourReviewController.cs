using Explorer.Stakeholders.Core.Domain;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist.Execution
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/tour-reviews")]
    [ApiController]
    public class TourReviewController : ControllerBase
    {
        private readonly ITourReviewService _tourReviewService;

        public TourReviewController(ITourReviewService tourReviewService)
        {
            _tourReviewService = tourReviewService;
        }

        private long GetTouristId()
        {
            var id = User.FindFirst("id")?.Value;
            if (id != null) return long.Parse(id);
            var pid = User.FindFirst("personId")?.Value;
            return long.Parse(pid ?? throw new System.Exception("No user id found"));
        }

        [HttpPost("{tourId}")]
        public ActionResult<TourReviewResponseDto> CreateReview(int tourId, [FromBody] TourReviewCreateDto reviewDto)
        {
            var result = _tourReviewService.CreateReview(tourId, GetTouristId(), reviewDto);
            return Ok(result);
        }

        [HttpPut("{tourId}")]
        public ActionResult<TourReviewResponseDto> UpdateReview(int tourId, [FromBody] TourReviewCreateDto reviewDto)
        {
            var result = _tourReviewService.UpdateReview(tourId, GetTouristId(), reviewDto);
            return Ok(result);
        }

        [HttpGet("{tourId}/eligibility")]
        public ActionResult<ReviewEligibilityDto> CheckEligibility(int tourId)
        {
            var result = _tourReviewService.CheckReviewEligibility(tourId, GetTouristId());
            return Ok(result);
        }

        [HttpGet("{tourId}")]
        public ActionResult<TourReviewResponseDto> GetReview(int tourId)
        {
            var result = _tourReviewService.GetReview(tourId, GetTouristId());
            return Ok(result);
        }
    }
}