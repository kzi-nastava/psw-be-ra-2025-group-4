using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Explorer.API.Controllers.Tourist; 

[Authorize(Policy = "touristPolicy")]
[ApiController]
[Route("api/tourist")]
public class GuideController : ControllerBase
{
    private readonly IGuideSelectionService _service;

    public GuideController(IGuideSelectionService service) => _service = service;

    private long GetTouristId()
    {
        var id = User.FindFirst("id")?.Value;
        if (!string.IsNullOrEmpty(id)) return long.Parse(id);

        var pid = User.FindFirst("personId")?.Value;
        if (!string.IsNullOrEmpty(pid)) return long.Parse(pid);

        throw new Exception("No user id found");
    }

    [HttpGet("tours/{tourId:int}/guides/available")]
    public IActionResult GetAvailable([FromRoute] int tourId, [FromQuery] long executionId)
    {
        var touristId = GetTouristId();
        var result = _service.GetAvailableGuides(tourId, executionId, touristId);
        return Ok(result);
    }

    [HttpPost("tour-execution/{executionId:long}/guide")]
    public IActionResult SelectGuide([FromRoute] long executionId, [FromBody] SelectGuideDto dto)
    {
        var touristId = GetTouristId();
        _service.SelectGuide(executionId, touristId, dto.GuideId);
        return NoContent();
    }

    [HttpDelete("tour-execution/{executionId:long}/guide")]
    public IActionResult CancelGuide([FromRoute] long executionId)
    {
        var touristId = GetTouristId();
        _service.CancelGuide(executionId, touristId);
        return NoContent();
    }
}
