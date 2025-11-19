using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [ApiController]
    [Route("api/ratings")]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        private long GetUserId()
        {
            var id = User.FindFirst("id")?.Value;
            
            if (id != null) return long.Parse(id);

            var pid = User.FindFirst("personId")?.Value;

            return long.Parse(pid ?? throw new Exception("No user id found"));

        }
        private bool IsAuthorOrTourist()
        {
            var role = User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

            if (role == null) return false;

            role = role.ToLower();

            return role == "tourist" || role == "author";
        }


        [Authorize(Policy = "administratorPolicy")]
        [HttpGet]
        public ActionResult<List<RatingDto>> GetAll()
        {
            var ratings = _ratingService.GetAll();
            return Ok(ratings);
        }

        [Authorize]
        [HttpGet("mine")]
        public ActionResult<List<RatingDto>> GetMine()
        {
            if (!IsAuthorOrTourist())
                return Forbid();

            var ratings = _ratingService.GetByUser(GetUserId());
            return Ok(ratings);
        }

        [Authorize]
        [HttpPost]
        public ActionResult<RatingDto> Create([FromBody] RatingCreateDto ratingCreateDto)
        {
            if (!IsAuthorOrTourist())
                return Forbid();

            var created = _ratingService.CreateRating(GetUserId(), ratingCreateDto);

            return Created(string.Empty, created);
        }

        [Authorize]
        [HttpPut("{id:long}")]
        public ActionResult<RatingDto> Update(long id, [FromBody] RatingUpdateDto ratingUpdateDto)
        {
            if (!IsAuthorOrTourist())
                return Forbid();

            var updated = _ratingService.UpdateRating(id, GetUserId(), ratingUpdateDto);
            
            return Ok(updated);
        }

        [Authorize]
        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            if (!IsAuthorOrTourist())
                return Forbid();

            _ratingService.DeleteRating(id, GetUserId());
            return NoContent();
        }
    }
}
