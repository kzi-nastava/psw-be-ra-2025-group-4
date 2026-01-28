using Explorer.API.Hubs;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Author;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/affiliate-codes")]
    [ApiController]
    public class AffiliateCodesController : ControllerBase
    {
        private readonly IAffiliateCodeService _service;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<MessageHub> _hub;
        private readonly ITourInfoService _tourInfoService;

        public AffiliateCodesController(IAffiliateCodeService service, INotificationService notificationService, IHubContext<MessageHub> hub, ITourInfoService tourInfoService)
        {
            _service = service;
            _notificationService = notificationService;
            _hub = hub;
            _tourInfoService = tourInfoService;
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
        public async Task<ActionResult<AffiliateCodeDto>> Create([FromBody] CreateAffiliateCodeDto dto)
        {
            var authorId = GetAuthorId();
            var created = _service.Create(dto, authorId);

            var partnerId = created.AffiliateTouristId;

            var authorUsername =
                User.FindFirst("username")?.Value ??
                User.Identity?.Name ??
                "Author";

            string content;
            string? resourceUrl = null;

            if (created.TourId.HasValue)
            {
                var tour = _tourInfoService.Get(created.TourId.Value);
                var tourName = tour?.Name ?? $"#{created.TourId.Value}";

                content = $"{authorUsername} assigned you an affiliate code for \"{tourName}\" ({created.Percent}%). Code: {created.Code}";
            }
            else
            {
                content = $"{authorUsername} assigned you a global affiliate code ({created.Percent}%). Code: {created.Code}";
            }

            var notification = _notificationService.CreateAffiliateCodeAssignedNotification(
                partnerUserId: partnerId,
                actorId: authorId,
                actorUsername: authorUsername,
                content: content,
                resourceUrl: resourceUrl
            );

            await _hub.Clients.Group($"user_{partnerId}")
                .SendAsync("ReceiveNotification", notification);

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
