using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
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
            return Ok(_tourReviewService.GetPagedByTour(tourId, page, pageSize));
        }

        [HttpGet("{id:int}")]
        public ActionResult<TourReviewDTO> GetById(int id)
        {
            var tourReview = _tourReviewService.GetById(id);
            return Ok(tourReview);
        }

        [HttpGet("my-review/{tourId:int}")]
        public ActionResult<TourReviewDTO> GetMyReviewForTour(int tourId)
        {
            var tourReview = _tourReviewService.GetByTouristAndTour(GetTouristId(), tourId);
            if (tourReview == null)
                return NotFound();
            return Ok(tourReview);
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

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            _tourReviewService.Delete(id);
            return NoContent();
        }
    }
}