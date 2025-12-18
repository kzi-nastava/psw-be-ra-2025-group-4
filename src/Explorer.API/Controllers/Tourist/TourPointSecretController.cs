using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/tour-points")]
[ApiController]
public class TourPointSecretController : ControllerBase
{
    private readonly ITourPointSecretService _secretService;

    public TourPointSecretController(ITourPointSecretService secretService)
    {
        _secretService = secretService;
    }

    private long GetTouristId()
    {
        var id = User.FindFirst("id")?.Value;
        if (!string.IsNullOrEmpty(id)) return long.Parse(id);

        var pid = User.FindFirst("personId")?.Value;
        if (!string.IsNullOrEmpty(pid)) return long.Parse(pid);

        throw new Exception("No user id found");
    }

    [HttpGet("{tourPointId:long}/secret")]
    public ActionResult<TourPointSecretDto> GetSecret(long tourPointId)
    {
        try
        {
            long touristId = GetTouristId();
            var result = _secretService.GetSecret(tourPointId, touristId);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

