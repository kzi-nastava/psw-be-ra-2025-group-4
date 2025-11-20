using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Message
{
    [Route("api/messages")]
    [ApiController]
    [Authorize]
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
            return Ok(_directMessageService.GetPaged(page, pageSize, User.PersonId()));
        }

        [HttpGet("conversations")]
        public ActionResult<PagedResult<DirectMessageDto>> GetAllConversations([FromQuery] int page, [FromQuery] int pageSize)
        {
            return Ok(_directMessageService.GetPagedConversations(page, pageSize, User.PersonId()));
        }


        [HttpPost]
        public ActionResult<DirectMessageDto> SendMessage([FromBody] DirectMessageDto directMessage)
        {
            try
            {
                var result = _directMessageService.SendMessage(User.PersonId(), directMessage);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("{id:long}")]
        public ActionResult<DirectMessageDto> Update([FromBody] DirectMessageDto directMessage)
        {
            try
            {
                var result = _directMessageService.Update(directMessage);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            try
            {
                _directMessageService.Delete(id);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
