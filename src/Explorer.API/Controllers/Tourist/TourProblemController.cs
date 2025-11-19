using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/tour-problems")]
[ApiController]
public class TourProblemController : ControllerBase
{
    private readonly ITourProblemService _tourProblemService;

    public TourProblemController(ITourProblemService tourProblemService)
    {
        _tourProblemService = tourProblemService;
    }

    private int GetTouristId()
    {
        var id = User.FindFirst("id")?.Value;
        if (id != null) return int.Parse(id);
        var pid = User.FindFirst("personId")?.Value;
        return int.Parse(pid ?? throw new System.Exception("No user id found"));
    }

    [HttpGet]
    public ActionResult<PagedResult<TourProblemDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_tourProblemService.GetPagedByTourist(GetTouristId(), page, pageSize));
    }

    [HttpGet("{id:int}")]
    public ActionResult<TourProblemDto> GetById(int id)
    {
        var tourProblem = _tourProblemService.GetById(id);
        return Ok(tourProblem);
    }

    [HttpPost]
    public ActionResult<TourProblemDto> Create([FromBody] TourProblemDto dto)
    {
        var created = _tourProblemService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public ActionResult<TourProblemDto> Update(int id, [FromBody] TourProblemDto dto)
    {
        dto.Id = id;
        var updated = _tourProblemService.Update(dto);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        _tourProblemService.Delete(id);
        return NoContent();
    }
}