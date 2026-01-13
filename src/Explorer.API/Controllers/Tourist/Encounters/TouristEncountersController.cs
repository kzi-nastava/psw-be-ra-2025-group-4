using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public.Administration;
using Explorer.Encounters.API.Public.Tourist;
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

        [HttpPost("{encounterId}/activate")]
        public IActionResult StartEncounter(long encounterId)
        {
            var touristId = GetTouristId();
            var encounter = _touristEncounterService.StartEncounter(touristId, encounterId);
            return Ok(encounter);
        }

    }
}
