using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/my-reviews")]
    [ApiController]
    public class TourReviewController : ControllerBase
    {
        private readonly ITourReviewService _tourReviewService;
        private readonly IUserService _userService;
        private readonly ITourService _tourService;

        public TourReviewController(
            ITourReviewService tourReviewService,
            IUserService userService,
            ITourService tourService)
        {
            _tourReviewService = tourReviewService;
            _userService = userService;
            _tourService = tourService;
        }

        private (int? userId, int? personId) GetIdsFromClaims()
        {
            int? Parse(string? s) => int.TryParse(s, out var v) ? v : null;

            var userId = Parse(User.FindFirst("id")?.Value);
            var personId = Parse(User.FindFirst("personId")?.Value);

            return (userId, personId);
        }

        [HttpGet("tour/{tourId:int}")]
        public ActionResult<PagedResult<TourReviewDTO>> GetByTour(
            int tourId,
            [FromQuery] int page,
            [FromQuery] int pageSize)
        {
            var tour = _tourService.GetById(tourId);
            if (tour == null) return NotFound();

            var (userId, personId) = GetIdsFromClaims();

            var allowed =
                (userId.HasValue && tour.AuthorId == userId.Value) ||
                (personId.HasValue && tour.AuthorId == personId.Value);

            if (!allowed)
                return Forbid();

            var result = _tourReviewService.GetPagedByTour(tourId, page, pageSize);

            foreach (var review in result.Results)
            {
                var u = _userService.GetById(review.TouristId);
                if (u != null) review.TouristUsername = u.Username;
            }

            return Ok(result);
        }
    }
}
