using System;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/preferences")]
    [ApiController]
    public class TourPreferencesController : ControllerBase
    {
        private readonly ITourPreferencesService _tourPreferencesService;

        public TourPreferencesController(ITourPreferencesService tourPreferencesService)
        {
            _tourPreferencesService = tourPreferencesService;
        }

        
        private bool TryGetCurrentTouristId(out int touristId)
        {
            touristId = 0;

            var idClaim = User.FindFirst("personId");
            if (idClaim == null) return false;

            return int.TryParse(idClaim.Value, out touristId);
        }

        [HttpGet]
        public ActionResult<TourPreferencesDTO> Get()
        {
            if (!TryGetCurrentTouristId(out var touristId))
                return Unauthorized();

            var prefs = _tourPreferencesService.GetForTourist(touristId);
            if (prefs == null)
                return NotFound();

            return Ok(prefs);
        }

        
        [HttpPut]
        public ActionResult<TourPreferencesDTO> Save([FromBody] TourPreferencesDTO dto)
        {
            if (dto == null)
                return BadRequest();

            if (!TryGetCurrentTouristId(out var touristId))
                return Unauthorized();

            var result = _tourPreferencesService.CreateOrUpdate(touristId, dto);
            return Ok(result);
        }

        
        [HttpDelete]
        public IActionResult Delete()
        {
            if (!TryGetCurrentTouristId(out var touristId))
                return Unauthorized();

            _tourPreferencesService.Delete(touristId);
            return NoContent();
        }
    }
}
