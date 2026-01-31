using System;
using System.Collections.Generic;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/affiliate-stats")]
    [ApiController]
    public class AffiliateStatsController : ControllerBase
    {
        private readonly IAffiliateStatsService _service;

        public AffiliateStatsController(IAffiliateStatsService service)
        {
            _service = service;
        }

        private int GetAuthorId()
        {
            var personId = User.FindFirst("personId")?.Value;
            if (!string.IsNullOrWhiteSpace(personId)) return int.Parse(personId);
            throw new Exception("No personId found in token.");
        }

        [HttpGet("summary")]
        public ActionResult<AffiliateDashboardSummaryDto> Summary(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            return Ok(_service.GetSummary(GetAuthorId(), from, to));
        }

        [HttpGet("by-tour")]
        public ActionResult<List<AffiliateTourStatsDto>> ByTour(
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            return Ok(_service.GetByTour(GetAuthorId(), from, to));
        }
    }
}
