using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/affiliate-codes")]
    [ApiController]
    public class AffiliateCodesController : ControllerBase
    {
        private readonly IAffiliateCodeService _service;

        public AffiliateCodesController(IAffiliateCodeService service)
        {
            _service = service;
        }

        private int GetAuthorId()
        {
            var personId = User.FindFirst("personId")?.Value;
            if (!string.IsNullOrWhiteSpace(personId)) return int.Parse(personId);
            throw new Exception("No personId found in token.");
        }

        [HttpGet]
        public ActionResult<List<AffiliateCodeDto>> GetAll([FromQuery] int? tourId = null)
        {
            return Ok(_service.GetAll(GetAuthorId(), tourId));
        }

        [HttpPost]
        public ActionResult<AffiliateCodeDto> Create([FromBody] CreateAffiliateCodeDto dto)
        {
            var created = _service.Create(dto, GetAuthorId());
            return Created(string.Empty, created);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Deactivate([FromRoute] int id)
        {
            _service.Deactivate(GetAuthorId(), id);
            return NoContent();
        }
    }
}
