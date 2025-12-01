using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/clubs/{clubId:long}/messages")]
    [ApiController]
    public class ClubMessagesController : ControllerBase
    {
        private readonly IClubMessageService _service;

        public ClubMessagesController(IClubMessageService service)
        {
            _service = service;
        }

        private long GetTouristId()
        {
            var id = User.FindFirst("id")?.Value;

            if (id != null) return long.Parse(id);

            var pid = User.FindFirst("personId")?.Value;
            return long.Parse(pid ?? throw new Exception("No user id found"));
        }

        [HttpGet]
        public ActionResult<List<ClubMessageDto>> Get(long clubId)
        {
            return Ok(_service.GetByClub(clubId));
        }

        [HttpPost]
        public ActionResult<ClubMessageDto> Create(long clubId, [FromBody] ClubMessageCreateDto dto)
        {
            long authorId = GetTouristId();
            var result = _service.Create(clubId, authorId, dto);
            return Created(string.Empty, result);
        }

        [HttpPut("{messageId:long}")]
        public ActionResult<ClubMessageDto> Update(long clubId, long messageId, [FromBody] ClubMessageCreateDto dto)
        {
            long authorId = GetTouristId();
            var result = _service.Update(messageId, authorId, dto);
            return Ok(result);
        }

        [HttpDelete("{messageId:long}")]
        public IActionResult Delete(long clubId, long messageId)
        {
            long ownerId = GetTouristId();
            _service.Delete(messageId, ownerId);
            return NoContent();
        }
    }
}
