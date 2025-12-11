using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/tours")]
[ApiController]
public class TourController : ControllerBase
{
    private readonly ITourService _tourService;

    public TourController(ITourService tourService)
    {
        _tourService = tourService;
    }

    private int GetAuthorId()
    {
        var id = User.FindFirst("id")?.Value;

        if (id != null) return int.Parse(id);

        var pid = User.FindFirst("personId")?.Value;

        return int.Parse(pid ?? throw new Exception("No user id found"));
    }

    [HttpGet]
    public ActionResult<PagedResult<TourDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_tourService.GetPagedByAuthor(GetAuthorId(), page, pageSize));
    }

    [HttpGet("{id:int}")]
    public ActionResult<TourDto> GetById(int id)
    {
        var tour = _tourService.GetByIdForAuthor(GetAuthorId(), id);
        return Ok(tour);
    }

    [HttpPost]
    public ActionResult<TourDto> Create([FromBody] CreateUpdateTourDto dto)
    {
        var created = _tourService.Create(dto, GetAuthorId());
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public ActionResult<TourDto> Update(int id, [FromBody] CreateUpdateTourDto dto)
    {
        var updated = _tourService.Update(id, dto, GetAuthorId());
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        _tourService.DeleteForAuthor(GetAuthorId(), id);
        return NoContent();
    }

    [HttpPut("{id:int}/price")]
    public ActionResult SetPrice(int id, [FromBody] decimal price)
    {
        _tourService.SetPrice(id, GetAuthorId(), price);
        return NoContent();
    }

    [HttpPut("{id:int}/equipment")]
    public ActionResult AddEquipment(int id, [FromBody] List<EquipmentDto> dtos)
    {
        _tourService.AddEquipment(id, GetAuthorId(), dtos);
        return NoContent();
    }

    [HttpPut("{id:int}/publish")]
    public ActionResult Publish(int id)
    {
        _tourService.Publish(id, GetAuthorId());
        return NoContent();
    }

    [HttpPut("{id:int}/archive")]
    public ActionResult Archive(int id)
    {
        _tourService.Archive(id, GetAuthorId());
        return NoContent();
    }

    [HttpPut("{id:int}/tour-point")]
    public ActionResult AddTourPoint(int id, [FromBody] TourPointDto tourPoint)
    {
        _tourService.AddTourPoint(id, GetAuthorId(), tourPoint);
        return NoContent();
    }

    public record UpdateRouteLengthRequest(double LengthInKm);

    [HttpPut("{id:int}/route-length")]
    public ActionResult<TourDto> UpdateRouteLength(int id, [FromBody] UpdateRouteLengthRequest request)
    {
        var updated = _tourService.UpdateRouteLength(id, GetAuthorId(), request.LengthInKm);
        return Ok(updated);
    }
}
