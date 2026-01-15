using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.API.Public.Administration;
using Explorer.Encounters.API.Public.Tourist;
using Explorer.Stakeholders.API.Dtos;
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
        private IEncounterParticipantService _encounterParticipantService;

        public TouristEncountersController(IEncounterService encounterService, ITouristEncounterService touristEncounterService, IEncounterParticipantService encounterParticipantService)
        {
            _encounterService = encounterService;
            _touristEncounterService = touristEncounterService;
            _encounterParticipantService = encounterParticipantService;
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

        [HttpPost("{encounterId:long}/social")]
        public ActionResult<int> UpdateSocialLocation([FromRoute] long encounterId, [FromBody] TouristLocationDto dto)
        {
            var activeCount = _touristEncounterService.UpdateTouristLocation(encounterId, GetTouristId(), dto.Latitude, dto.Longitude);
            return Ok(activeCount);
        }

        [HttpPost("social")]
        public ActionResult<SocialEncounterDto> CreateSocial([FromBody] SocialEncounterDto dto)
        {
            if (_encounterParticipantService.GetLevel(GetTouristId()) < 10)
                throw new InvalidOperationException("You have to be atleast level 10 to create encounters!");

            var result = _encounterService.CreateSocial(dto, true);
            return Ok(result);
        }

        [HttpPost("hidden-location")]
        public ActionResult<HiddenLocationEncounterDto> CreateHiddenLocation([FromBody] HiddenLocationEncounterDto dto)
        {
            if (_encounterParticipantService.GetLevel(GetTouristId()) < 10)
                throw new InvalidOperationException("You have to be atleast level 10 to create encounters!");
            var result = _encounterService.CreateHiddenLocation(dto, true);
            return Ok(result);
        }

        [HttpPost("misc")]
        public ActionResult<EncounterDto> CreateMisc([FromBody] EncounterDto dto)
        {
            if (_encounterParticipantService.GetLevel(GetTouristId()) < 10)
                throw new InvalidOperationException("You have to be atleast level 10 to create encounters!");
            var result = _encounterService.Create(dto, true);
            return Ok(result);
        }

        [HttpPut("misc/{id:long}")]
        public ActionResult<EncounterDto> UpdateMisc(long id, [FromBody] EncounterUpdateDto dto)
        {
            dto.Id = id;
            var updated = _encounterService.Update(dto, (int)id);
            return Ok(updated);
        }

        [HttpPut("social/{id:long}")]
        public ActionResult<SocialEncounterDto> UpdateSocial(long id, [FromBody] SocialEncounterDto dto)
        {
            var updated = _encounterService.UpdateSocial(dto, (int)id);
            return Ok(updated);
        }

        [HttpPut("hidden-location/{id:long}")]
        public ActionResult<HiddenLocationEncounterDto> UpdateHiddenLocation(long id, [FromBody] HiddenLocationEncounterDto dto)
        {
            var updated = _encounterService.UpdateHiddenLocation(dto, (int)id);
            return Ok(updated);
        }

        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            _encounterService.Delete(id);
            return NoContent();
        }
    }
}
