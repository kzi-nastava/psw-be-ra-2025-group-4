using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.API.Public.Administration;
using Explorer.Encounters.API.Public.Tourist;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Explorer.API.Controllers.Tourist.Encounters
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/encounters")]
    [ApiController]
    public class TouristEncountersController : ControllerBase
    {
        private IEncounterService _encounterService;
        private ITouristEncounterService _touristEncounterService;

        public TouristEncountersController(IEncounterService encounterService, ITouristEncounterService touristEncounterService)
        {
            _encounterService = encounterService;
            _touristEncounterService = touristEncounterService;
        }
        private long GetTouristId()
        {
            var id = User.FindFirst("id")?.Value;

            if (id != null) return long.Parse(id);

            var pid = User.FindFirst("personId")?.Value;

            return long.Parse(pid ?? throw new Exception("No user id found"));
        }

        [HttpGet]
        public ActionResult<IEnumerable<EncounterDto>> GetActive()
        {
            return Ok(_encounterService.GetActive());
        }

        [HttpGet("by-tourpoint/{tourPointId:int}")]
        public ActionResult<List<EncounterViewDto>> GetByTourPoint(
            [FromRoute] int tourPointId,
            [FromQuery] double latitude,
            [FromQuery] double longitude)
        {
            var touristId = GetTouristId();

            var location = new LocationDto
            {
                Latitude = latitude,
                Longitude = longitude
            };

            var result = _touristEncounterService.GetByTourPoint(touristId, tourPointId, location);
            return Ok(result);
        }

        [HttpPost("{id:long}/activate")]
        public ActionResult Activate([FromRoute] long id)
        {
            var touristId = GetTouristId();
            _touristEncounterService.StartEncounter(touristId, id);
            return Ok();
        }

        [HttpPost("{id:long}/location")]
        public ActionResult UpdateLocation([FromRoute] long id, [FromBody] LocationDto location)
        {
            var touristId = GetTouristId();

            _touristEncounterService.UpdateLocation(touristId, id, location);
            return Ok();
        }
        [HttpPost("{id:long}/complete")]
        public ActionResult Complete([FromRoute] long id)
        {
            var touristId = GetTouristId();
            _touristEncounterService.CompleteEncounter(touristId, id);
            return Ok();
        }
    }
}
