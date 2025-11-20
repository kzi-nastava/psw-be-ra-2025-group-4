using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/digital-diaries")]
    [ApiController]
    public class DigitalDiariesController : ControllerBase
    {
        private readonly IDigitalDiaryService _digitalDiaryService;

        public DigitalDiariesController(IDigitalDiaryService digitalDiaryService)
        {
            _digitalDiaryService = digitalDiaryService;
        }

        [HttpGet]
        public ActionResult<PagedResult<DigitalDiaryDto>> GetMine([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var touristId = User.PersonId();
            var result = _digitalDiaryService.GetPagedByTourist(touristId, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public ActionResult<DigitalDiaryDto> GetById(long id)
        {
            var result = _digitalDiaryService.GetById(id);
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<DigitalDiaryDto> Create([FromBody] DigitalDiaryDto dto)
        {
            dto.TouristId = User.PersonId();
            var created = _digitalDiaryService.Create(dto);
            return Ok(created);
        }

        [HttpPut("{id:long}")]
        public ActionResult<DigitalDiaryDto> Update(long id, [FromBody] DigitalDiaryDto dto)
        {
            dto.Id = id;
            dto.TouristId = User.PersonId();
            var updated = _digitalDiaryService.Update(dto);
            return Ok(updated);
        }

        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            _digitalDiaryService.Delete(id);
            return Ok();
        }
    }
}
