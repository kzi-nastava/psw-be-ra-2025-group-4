using System.Collections.Generic;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Author;
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

    [HttpGet]
    public ActionResult<PagedResult<TourDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        var result = _tourService.GetPaged(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public ActionResult<TourDto> GetById(int id)
    {
        var tour = _tourService.GetById(id);
        if (tour == null)
            return NotFound();

        return Ok(tour);
    }

    [HttpPost]
    public ActionResult<TourDto> Create([FromBody] TourDto tour)
    {
        if (tour == null)
            return BadRequest();

        var createdTour = _tourService.Create(tour);
        return CreatedAtAction(nameof(GetById), new { id = createdTour.Id }, createdTour);
    }

    [HttpPut("{id:int}")]
    public ActionResult<TourDto> Update(int id, [FromBody] TourDto tour)
    {
        if (tour == null || id != tour.Id)
            return BadRequest();

        var updatedTour = _tourService.Update(tour);
        if (updatedTour == null)
            return NotFound();

        return Ok(updatedTour);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var exists = _tourService.GetById(id);
        if (exists == null)
            return NotFound();

        _tourService.Delete(id);
        return NoContent();
    }
}
