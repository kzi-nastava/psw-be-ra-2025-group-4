using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/tourists")]
    [ApiController]
    public class AuthorTouristsController : ControllerBase
    {
        private readonly ITouristLookupService _service;

        public AuthorTouristsController(ITouristLookupService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<TouristLookupDto>> GetAll([FromQuery] bool onlyActive = true)
        {
            return Ok(_service.GetAll(onlyActive));
        }
    }
}
