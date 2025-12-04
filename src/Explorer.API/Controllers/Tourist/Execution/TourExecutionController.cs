using System;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist.Execution;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/tour-execution")]
[ApiController]
public class TourExecutionController : ControllerBase
{
    private readonly ITourExecutionService _tourExecutionService;

    public TourExecutionController(ITourExecutionService tourExecutionService)
    {
        _tourExecutionService = tourExecutionService;
    }

    [HttpPost("start")]
    public ActionResult<TourExecutionDto> StartTour([FromBody] TourExecutionCreateDto dto)
    {
        try
        {
            long touristId = User.PersonId();
            var result = _tourExecutionService.StartTour(dto, touristId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{executionId:long}/complete")]
    public ActionResult<TourExecutionDto> Complete(long executionId)
    {
        try
        {
            long touristId = User.PersonId();
            var result = _tourExecutionService.Complete(executionId, touristId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpPut("{executionId:long}/abandon")]
    public ActionResult<TourExecutionDto> Abandon(long executionId)
    {
        try
        {
            long touristId = User.PersonId();
            var result = _tourExecutionService.Abandon(executionId, touristId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpGet("{executionId:long}")]
    public ActionResult<TourExecutionDto> GetById(long executionId)
    {
        try
        {
            long touristId = User.PersonId();
            var result = _tourExecutionService.GetById(executionId, touristId);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpPut("{executionId:long}/track")]
    public ActionResult<TourExecutionDto> Track(
    long executionId,
    [FromBody] TourExecutionTrackDto dto)
    {
        var touristId = User.PersonId();
        return Ok(_tourExecutionService.Track(executionId, touristId, dto));
    }

}

