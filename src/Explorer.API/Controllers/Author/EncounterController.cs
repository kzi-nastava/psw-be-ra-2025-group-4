using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public.Administration;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/encounters")]
    [ApiController]
    public class EncounterController : ControllerBase
    {
        private readonly IEncounterService _encounterService;
        private readonly ITourService _tourService;

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


        [HttpGet("by-tour/{tourId:int}")]
        public ActionResult<List<EncounterDto>> GetByTourId(int tourId)
        {
            var tour = _tourService.GetById(tourId);
            if (tour == null)
            {
                return NotFound($"Tour with id {tourId} not found.");
            }

            var tourPointIds = tour.Points.Select(p => p.Id).ToList();
            var encounters = _encounterService.GetByTourPointIds(tourPointIds);
            return Ok(encounters);
        }


        [HttpPost]
        public ActionResult<EncounterDto> Create([FromBody] EncounterDto dto)
        {
            var created = _encounterService.Create(dto);
            if (dto.TourPointId != null)
                _encounterService.AddEncounterToTourPoint(created.Id, (long)dto.TourPointId, dto.IsRequiredForPointCompletion ?? false);

            return CreatedAtAction(nameof(GetPaged), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public ActionResult<EncounterDto> Update([FromBody] EncounterUpdateDto dto, int id)
        {
            var updated = _encounterService.Update(dto, id);
            return Ok(updated);
        }

        [HttpPut("publish/{id:int}")]
        public IActionResult Publish(int id)
        {
            _encounterService.Publish(id);
            return NoContent();
        }

        [HttpPut("archive/{id:int}")]
        public IActionResult Archive(int id)
        {
            _encounterService.Archive(id);
            return NoContent();
        }

        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            _encounterService.Delete(id);
            return NoContent();
        }

        private int GetAuthorId()
        {
            var id = User.FindFirst("id")?.Value;

            if (id != null) return int.Parse(id);

            var pid = User.FindFirst("personId")?.Value;

            return int.Parse(pid ?? throw new Exception("No user id found"));
        }
    }
}
