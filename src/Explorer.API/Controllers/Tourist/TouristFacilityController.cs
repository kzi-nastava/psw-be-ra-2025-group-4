using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/facility")]
    [ApiController]
    public class TouristFacilityController : ControllerBase
    {
        private readonly IFacilityService _service;

        public TouristFacilityController(IFacilityService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<PagedResult<FacilityDto>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 100)
        {
            return Ok(_service.GetPaged(page, pageSize));
        }
    }
}
