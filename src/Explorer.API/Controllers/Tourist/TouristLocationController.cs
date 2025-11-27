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

        [HttpGet]
        public ActionResult<TouristLocationDto> GetMine()
        {
            var result = _service.Get(User.PersonId());
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<TouristLocationDto> Update([FromBody] TouristLocationDto dto)
        {
            var updated = _service.SaveOrUpdateLocation(User.PersonId(), dto);
            return Ok(updated);
        }
    }
}
