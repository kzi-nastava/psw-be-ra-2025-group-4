using System;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Encounters.API.Public.Administration;
using Explorer.Encounters.API.Public.Tourist;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
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
    private readonly ITouristEncounterService _touristEncounterService;

    public TourExecutionController(ITourExecutionService tourExecutionService, ITouristEncounterService touristEncounterService)
    {
        _tourExecutionService = tourExecutionService;
        _touristEncounterService = touristEncounterService;
    }

    [HttpPost("start")]
    public ActionResult<TourExecutionDto> StartTour([FromBody] TourExecutionCreateDto dto)
    {
        try
        {
            long touristId = GetTouristId();
            var result = _tourExecutionService.StartTour(dto, touristId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private long GetTouristId()
    {
        var id = User.FindFirst("id")?.Value;
        if (!string.IsNullOrEmpty(id)) return long.Parse(id);

        var pid = User.FindFirst("personId")?.Value;
        if (!string.IsNullOrEmpty(pid)) return long.Parse(pid);

        throw new Exception("No user id found");
    }

    [HttpPut("{executionId:long}/complete")]
    public ActionResult<TourExecutionDto> Complete(long executionId)
    {
        try
        {
            long touristId = GetTouristId();
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
            long touristId = GetTouristId();
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
            long touristId = GetTouristId();
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
        var touristId = GetTouristId();
        var execution = _tourExecutionService.GetById(executionId, touristId);

        var isEncounterCompleted = _touristEncounterService.IsEncounterCompleted(execution.NextKeyPoint.Id, touristId);
        return Ok(_tourExecutionService.Track(executionId, touristId, dto, isEncounterCompleted));
    }


    [HttpGet("active/tour/{tourId:int}")]
    public ActionResult<TourExecutionDto> GetActiveByTour(int tourId)
    {
        try
        {
            long touristId = GetTouristId();
            var result = _tourExecutionService.GetActiveByTour(tourId, touristId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

