using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers;

[AllowAnonymous]
[Route("api/tour-reviews")]
[ApiController]
public class PublicTourReviewController : ControllerBase
{
    private readonly ITourReviewService _tourReviewService;
    private readonly IUserService _userService;

    public PublicTourReviewController(ITourReviewService tourReviewService, IUserService userService)
    {
        _tourReviewService = tourReviewService;
        _userService = userService;
    }

    [HttpGet("tour/{tourId:int}")]
    public ActionResult<PagedResult<TourReviewDTO>> GetByTour(int tourId, [FromQuery] int page, [FromQuery] int pageSize)
    {
        var result = _tourReviewService.GetPagedByTour(tourId, page, pageSize);

        foreach (var tourReview in result.Results)
        {
            UserDto? u = _userService.GetById(tourReview.TouristId);
            if (u == null) continue;
            tourReview.TouristUsername = u.Username;
        }

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public ActionResult<TourReviewDTO> GetById(int id)
    {
        var tourReview = _tourReviewService.GetById(id);

        var u = _userService.GetById(tourReview.TouristId);
        if (u != null) tourReview.TouristUsername = u.Username;

        return Ok(tourReview);
    }
}
