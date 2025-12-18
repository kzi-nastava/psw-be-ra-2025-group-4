using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/tour-reviews")]
    [ApiController]
    public class TourReviewController : ControllerBase
    {
        private readonly ITourReviewService _tourReviewService;
        private readonly IUserService _userService;

        public TourReviewController(ITourReviewService tourReviewService, IUserService userService)
        {
            _tourReviewService = tourReviewService;
            _userService = userService;
        }

        private int GetTouristId()
        {
            var id = User.FindFirst("id")?.Value;
            if (id != null) return int.Parse(id);
            var pid = User.FindFirst("personId")?.Value;
            return int.Parse(pid ?? throw new System.Exception("No user id found"));
        }

        [HttpGet]
        public ActionResult<PagedResult<TourReviewDTO>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        {
            return Ok(_tourReviewService.GetPagedByTourist(GetTouristId(), page, pageSize));
        }

        [HttpGet("tour/{tourId:int}")]
        public ActionResult<PagedResult<TourReviewDTO>> GetByTour(int tourId, [FromQuery] int page, [FromQuery] int pageSize)
        {
            var result = _tourReviewService.GetPagedByTour(tourId, page, pageSize);
            foreach (var tour in result.Results)
            {
                UserDto? u = _userService.GetById(tour.TouristId);
                if (u == null) continue;
                tour.TouristUsername = u.Username;
            }
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public ActionResult<TourReviewDTO> GetById(int id)
        {
            var tourReview = _tourReviewService.GetById(id);
            return Ok(tourReview);
        }

        [HttpGet("eligibility/{tourId:int}")]
        public ActionResult<object> CheckEligibility(int tourId)
        {
            var touristId = GetTouristId();

            try
            {
                var existingReview = _tourReviewService.GetByTouristAndTour(touristId, tourId);
                var eligibilityInfo = _tourReviewService.GetReviewEligibilityInfo(touristId, tourId);

                return Ok(new
                {
                    canLeaveReview = eligibilityInfo.CanLeaveReview,
                    reason = eligibilityInfo.Reason,
                    completionPercentage = eligibilityInfo.CompletionPercentage,
                    daysSinceLastActivity = eligibilityInfo.DaysSinceLastActivity,
                    existingReview = existingReview
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    canLeaveReview = false,
                    reason = ex.Message,
                    completionPercentage = 0.0,
                    daysSinceLastActivity = 0.0,
                    existingReview = (TourReviewDTO)null
                });
            }
        }

        [HttpPost]
        public ActionResult<TourReviewDTO> Create([FromBody] TourReviewDTO dto)
        {
            dto.TouristId = GetTouristId();
            var created = _tourReviewService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public ActionResult<TourReviewDTO> Update(int id, [FromBody] TourReviewDTO dto)
        {
            dto.Id = id;
            dto.TouristId = GetTouristId();
            var updated = _tourReviewService.Update(dto);
            return Ok(updated);
        }
    }
}