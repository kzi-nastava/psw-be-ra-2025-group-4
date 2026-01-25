using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/favorite-tours")]
    [ApiController]
    public class FavoriteTourController : ControllerBase
    {
        private readonly IFavoriteTourService _favoriteTourService;

        public FavoriteTourController(IFavoriteTourService favoriteTourService)
        {
            _favoriteTourService = favoriteTourService;
        }

        private int GetTouristId()
        {
            var id = User.FindFirst("id")?.Value;
            if (id != null) return int.Parse(id);
            var pid = User.FindFirst("personId")?.Value;
            return int.Parse(pid ?? throw new System.Exception("No user id found"));
        }

        [HttpPost("{tourId:int}")]
        public ActionResult<FavoriteTourDto> AddFavorite(int tourId)
        {
            var touristId = GetTouristId();
            var result = _favoriteTourService.AddFavorite(touristId, tourId);
            return Ok(result);
        }

        [HttpDelete("{tourId:int}")]
        public ActionResult RemoveFavorite(int tourId)
        {
            var touristId = GetTouristId();
            _favoriteTourService.RemoveFavorite(touristId, tourId);
            return NoContent();
        }

        [HttpGet("{tourId:int}/is-favorite")]
        public ActionResult<bool> IsFavorite(int tourId)
        {
            var touristId = GetTouristId();
            var result = _favoriteTourService.IsFavorite(touristId, tourId);
            return Ok(result);
        }

        [HttpGet]
        public ActionResult<PagedResult<TourDto>> GetFavoriteTours([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var touristId = GetTouristId();
            var result = _favoriteTourService.GetFavoriteTours(touristId, page, pageSize);
            return Ok(result);
        }
    }
}
