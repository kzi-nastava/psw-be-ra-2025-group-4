using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers;

[AllowAnonymous]
[Route("api/tours")]
[ApiController]
public class PublicTourController : ControllerBase
{
    private readonly ITourService _tourService;
    private readonly ITourReviewService _tourReviewService;

    public PublicTourController(ITourService tourService, ITourReviewService tourReviewService)
    {
        _tourService = tourService;
        _tourReviewService = tourReviewService;
    }

    [HttpGet("{id:int}")]
    public ActionResult<TourDto> GetById(int id)
    {
        var tour = _tourService.GetById(id);
        tour.AverageGrade = _tourReviewService.GetTourAverageGrade(id);
        return Ok(tour);
    }

    [HttpGet]
    public ActionResult<PagedResult<TourDto>> GetAll(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? search,
        [FromQuery] int? difficulty,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] List<string>? tags,
        [FromQuery] string? sort)
    {
        var result = _tourService.GetPublishedFiltered(
            page, pageSize,
            search, difficulty,
            minPrice, maxPrice,
            tags, sort);

        foreach (var tour in result.Results)
            tour.AverageGrade = _tourReviewService.GetTourAverageGrade(tour.Id);

        return Ok(result);
    }

    [HttpGet("tags")]
    public ActionResult<IEnumerable<string>> GetTags()
        => Ok(_tourService.GetAllTags());
}
