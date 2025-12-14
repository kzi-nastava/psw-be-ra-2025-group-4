using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Message
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Vraca sve notifikacije korisnika (paged).
        /// </summary>
        [HttpGet]
        public ActionResult<PagedResult<NotificationDto>> GetPaged(
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 10)
        {
            long userId = User.PersonId();
            var result = _notificationService.GetPaged(userId, page, pageSize);

            return Ok(result);
        }

        /// <summary>
        /// Oznacava jednu notifikaciju kao procitanu.
        /// </summary>
        [HttpPut("{id:long}/read")]
        public IActionResult MarkAsRead(long id)
        {
            _notificationService.MarkAsRead(id);
            return Ok();
        }

        /// <summary>
        /// Oznacava sve notifikacije korisnika kao procitane.
        /// </summary>
        [HttpPut("read-all")]
        public IActionResult MarkAll()
        {
            long userId = User.PersonId();
            _notificationService.MarkAll(userId);

            return Ok();
        }

        [HttpPut("messages/from/{actorId:long}/read")]
        public IActionResult MarkConversationAsRead(long actorId)
        {
            long userId = User.PersonId();
            _notificationService.MarkConversationAsRead(userId, actorId);
            return Ok();
        }

    }
}
