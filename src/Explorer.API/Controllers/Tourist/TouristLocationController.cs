using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/location")]
    [ApiController]
    public class TouristLocationController : ControllerBase
    {
        private readonly ITouristLocationService _service;

        public TouristLocationController(ITouristLocationService service)
        {
            _service = service;
        }
        private long GetTouristId()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "personId")?.Value;
            if (string.IsNullOrWhiteSpace(idClaim))
                throw new UnauthorizedAccessException("Missing personId claim.");

            return long.Parse(idClaim);
        }

        [HttpGet]
        public ActionResult<TouristLocationDto> GetMine()
        {
            var userId = GetTouristId();
            var result = _service.Get(userId);
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<TouristLocationDto> Update([FromBody] TouristLocationDto dto)
        {
            var userId = GetTouristId();
            var updated = _service.SaveOrUpdateLocation(userId, dto);
            return Ok(updated);
        }
    }
}
