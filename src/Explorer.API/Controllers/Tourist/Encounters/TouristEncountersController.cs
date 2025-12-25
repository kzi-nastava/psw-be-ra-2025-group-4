using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public.Administration;
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

        public TouristEncountersController(IEncounterService encounterService)
        {
            _encounterService = encounterService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<EncounterDto>> GetActive()
        {
            return Ok(_encounterService.GetActive());
        }
    }
}
