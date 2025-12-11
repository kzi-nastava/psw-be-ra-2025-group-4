using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.API.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/messages")]
    [ApiController]
    public class AuthorDirectMessageController : ControllerBase
    {
        private readonly IDirectMessageService _directMessageService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<MessageHub> _hubContext;

        public AuthorDirectMessageController(
            IDirectMessageService directMessageService,
            INotificationService notificationService,
            IHubContext<MessageHub> hubContext)
        {
            _directMessageService = directMessageService;
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        [HttpGet("conversations")]
        public ActionResult<PagedResult<DirectMessageDto>> GetConversations(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            return Ok(_directMessageService.GetPagedConversations(page, pageSize, GetUserId()));
        }

        [HttpGet("history/{otherUserId}")]
        public ActionResult<PagedResult<DirectMessageDto>> GetMessageHistory(
            long otherUserId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            return Ok(_directMessageService.GetPagedBetweenUsers(page, pageSize, GetUserId(), otherUserId));
        }

        // ✅ SEND MESSAGE + NOTIFICATION
        [HttpPost]
        public async Task<ActionResult<DirectMessageDto>> SendMessage(
            [FromBody] DirectMessageDto messageDto)
        {
            try
            {
                var result = _directMessageService.SendMessage(GetUserId(), messageDto);

                // ➊ Kreiraj notifikaciju
                var notification = _notificationService.CreateMessageNotification(
                    result.RecipientId,
                    result.Content,
                    result.ResourceUrl
                );

                // ➋ Pošalji real-time SignalR notifikaciju
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

        [HttpPost("start")]
        public async Task<ActionResult<DirectMessageDto>> StartConversation(
            [FromBody] ConversationStartDto messageDto)
        {
            try
            {
                var result = _directMessageService.StartConversation(GetUserId(), messageDto);

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


        [HttpPut]
        public ActionResult<DirectMessageDto> UpdateMessage(
            [FromBody] DirectMessageDto messageDto)
        {
            try
            {
                var result = _directMessageService.Update(messageDto);
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

        [HttpDelete("{id}")]
        public ActionResult DeleteMessage(long id)
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

        private int GetUserId()
        {
            var id = User.FindFirst("id")?.Value;
            if (id != null) return int.Parse(id);

            var pid = User.FindFirst("personId")?.Value;
            return int.Parse(pid ?? throw new Exception("No user id found"));
        }
    }
}
