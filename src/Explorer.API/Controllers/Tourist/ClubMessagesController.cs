using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.API.Hubs;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.AspNetCore.SignalR;


namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/clubs/{clubId:long}/messages")]
    [ApiController]
    public class ClubMessagesController : ControllerBase
    {
        private readonly IClubMessageService _service;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly IUserRepository _userRepository;


        public ClubMessagesController(IClubMessageService service, INotificationService notificationService, IHubContext<MessageHub> hubContext, IUserRepository userRepository)
        {
            _service = service;
            _notificationService = notificationService;
            _hubContext = hubContext;
            _userRepository = userRepository;
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
        public async Task<ActionResult<ClubMessageDto>> Create(long clubId, [FromBody] ClubMessageCreateDto dto)
        {
            long authorId = GetTouristId();
            var result = _service.Create(clubId, authorId, dto);

            var author = _userRepository.Get(authorId);
            var authorUsername = author?.Username ?? "Unknown";

            var tourists = _userRepository.GetAllActiveTourists();
            foreach (var t in tourists.Where(x => x.Id != authorId))
            {
                var notif = _notificationService.CreateClubNotification(
                    userId: t.Id,
                    content: dto.Text,
                    actorId: authorId,
                    actorUsername: authorUsername,
                    clubId: clubId
                );

                await _hubContext.Clients
                    .Group($"user_{t.Id}")
                    .SendAsync("ReceiveNotification", notif);
            }

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
