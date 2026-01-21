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
            var id = User.FindFirst("id")?.Value;
            if (id != null) return int.Parse(id);

            var pid = User.FindFirst("personId")?.Value;
            return int.Parse(pid ?? throw new Exception("No user id found"));
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
    }
}
