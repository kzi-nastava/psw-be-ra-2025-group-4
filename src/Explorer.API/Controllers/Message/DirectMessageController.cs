using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Explorer.API.Hubs;
using Explorer.Stakeholders.API.Public;

namespace Explorer.API.Controllers.Message
{
    [Route("api/messages")]
    [ApiController]
    [Authorize]
    public class DirectMessageController : ControllerBase
    {
        private readonly IDirectMessageService _directMessageService;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly INotificationService _notificationService;


        public DirectMessageController(
            IDirectMessageService directMessageService,
            IHubContext<MessageHub> hubContext,
            INotificationService notificationService)
        {
            _directMessageService = directMessageService;
            _hubContext = hubContext;
            _notificationService = notificationService;
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

        [HttpGet("conversations/{userId:long}")]
        public ActionResult<PagedResult<DirectMessageDto>> GetAllBetweenUsers(long userId, [FromQuery] int page, [FromQuery] int pageSize)
        {
            return Ok(_directMessageService.GetPagedBetweenUsers(page, pageSize, User.PersonId(), userId));
        }

        [HttpPost("start")]
        public async Task<ActionResult<DirectMessageDto>> StartConversation(
            [FromBody] ConversationStartDto messageDto)
        {
            try
            {
                var result = _directMessageService.StartConversation(User.PersonId(), messageDto);

                var notification = _notificationService.CreateMessageNotification(
                    result.RecipientId,
                    result.Content,
                    result.ResourceUrl
                );

                await _hubContext.Clients
                    .Group($"user_{result.RecipientId}")
                    .SendAsync("ReceiveNotification", notification);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        public async Task<ActionResult<DirectMessageDto>> SendMessage([FromBody] DirectMessageDto directMessage)
        {
            try
            {
                var result = _directMessageService.SendMessage(User.PersonId(), directMessage);

                // ➤➤➤ DODAJ OVO: kreiranje notifikacije
                var notification = _notificationService.CreateMessageNotification(
                    result.RecipientId,
                    result.Content,
                    result.ResourceUrl
                );

                // ➤➤➤ DODAJ OVO: slanje SignalR notifikacije
                await _hubContext.Clients
                    .Group($"user_{result.RecipientId}")
                    .SendAsync("ReceiveNotification", notification);

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
