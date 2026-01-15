using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration
{
    [Authorize(Policy = "administratorPolicy")]
    [Route("api/administration/encounter")]
    [ApiController]
    public class EncounterController : ControllerBase
    {
        private readonly IEncounterService _encounterService;

        public EncounterController(IEncounterService encounterService)
        {
            _encounterService = encounterService;
        }

        [HttpGet]
        public ActionResult<PagedResult<EncounterDto>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var result = _encounterService.GetPaged(page, pageSize);
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<EncounterDto> Create([FromBody] EncounterDto dto)
        {
            var created = _encounterService.Create(dto, false);
            return CreatedAtAction(nameof(GetPaged), new { id = created.Id }, created);
        }

        [HttpPost("hidden")]
        public ActionResult<HiddenLocationEncounterDto> CreateHidden([FromBody] HiddenLocationEncounterDto dto)
        {
            var result = _encounterService.CreateHiddenLocation(dto, false);
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public ActionResult<EncounterDto> Update([FromBody] EncounterUpdateDto dto, int id)
        {
            var updated = _encounterService.Update(dto, id);
            return Ok(updated);
        }

        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            _encounterService.Delete(id);
            return NoContent();
        }

        [HttpPut("approve/{id:long}")]
        public IActionResult Approve(long id)
        {
            _encounterService.Approve(id);
            return NoContent();
        }


        [HttpPut("decline/{id:long}")]
        public IActionResult Decline(long id)
        {
            _encounterService.Decline(id);
            return NoContent();
        }
    }
}
