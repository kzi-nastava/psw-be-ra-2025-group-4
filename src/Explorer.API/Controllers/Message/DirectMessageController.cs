using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Message
{
    [Route("api/messages")]
    [ApiController]
    public class DirectMessageController : ControllerBase
    {
        private readonly IDirectMessageService _directMessageService;

        public DirectMessageController(IDirectMessageService directMessageService)
        {
            _directMessageService = directMessageService;
        }

        [HttpGet]
        public ActionResult<PagedResult<DirectMessageDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        {
            return Ok(_directMessageService.GetPaged(page, pageSize, User?.Identity?.Name));
        }

        [HttpGet("conversations")]
        public ActionResult<PagedResult<DirectMessageDto>> GetAllConversations([FromQuery] int page, [FromQuery] int pageSize)
        {
            return Ok(_directMessageService.GetPagedConversations(page, pageSize, User?.Identity?.Name));
        }


        [HttpPost]
        public ActionResult<DirectMessageDto> SendMessage([FromBody] DirectMessageDto directMessage)
        {
            return Ok(_directMessageService.SendMessage(directMessage));
        }

        [HttpPut("{id:long}")]
        public ActionResult<DirectMessageDto> Update([FromBody] DirectMessageDto directMessage)
        {
            return Ok(_directMessageService.Update(directMessage));
        }

        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            _directMessageService.Delete(id);
            return Ok();
        }
    }
}
