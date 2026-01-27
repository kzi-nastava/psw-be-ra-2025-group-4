using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Explorer.API.Hubs;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Explorer.API.Controllers.Tourist.Payments
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/group-travel")]
    [ApiController]
    public class GroupTravelController : ControllerBase
    {
        private readonly IGroupTravelService _groupTravelService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<MessageHub> _hubContext;

        public GroupTravelController(
            IGroupTravelService groupTravelService,
            INotificationService notificationService,
            IHubContext<MessageHub> hubContext)
        {
            _groupTravelService = groupTravelService;
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        private int GetTouristId()
        {
            var id = User.FindFirst("id")?.Value;
            if (!string.IsNullOrWhiteSpace(id)) return int.Parse(id);

            var pid = User.FindFirst("personId")?.Value;
            if (!string.IsNullOrWhiteSpace(pid)) return int.Parse(pid);

            throw new Exception("No user id found");
        }

        [HttpPost]
        public ActionResult<GroupTravelRequestDto> Create([FromBody] CreateGroupTravelRequestDto dto)
        {
            try
            {
                var organizerId = GetTouristId();
                var result = _groupTravelService.Create(organizerId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("my-requests")]
        public ActionResult<List<GroupTravelRequestDto>> GetMyRequests()
        {
            try
            {
                var touristId = GetTouristId();
                var requests = _groupTravelService.GetMyRequests(touristId);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("requests-for-me")]
        public ActionResult<List<GroupTravelRequestDto>> GetRequestsForMe()
        {
            try
            {
                var touristId = GetTouristId();
                var requests = _groupTravelService.GetRequestsForMe(touristId);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{requestId:int}/accept")]
        public async Task<ActionResult<GroupTravelRequestDto>> AcceptRequest(int requestId)
        {
            try
            {
                var participantId = GetTouristId();
                var result = _groupTravelService.AcceptRequest(requestId, participantId);

                var notification = _notificationService.CreateMessageNotification(
                    userId: result.OrganizerId,
                    actorId: participantId,
                    actorUsername: "System",
                    content: $"A participant accepted your group travel request: {result.TourName}",
                    resourceUrl: $"/group-travel/requests/{requestId}"
                );

                await _hubContext.Clients
                    .Group($"user_{result.OrganizerId}")
                    .SendAsync("ReceiveNotification", notification);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{requestId:int}/reject")]
        public async Task<ActionResult<GroupTravelRequestDto>> RejectRequest(int requestId)
        {
            try
            {
                var participantId = GetTouristId();
                var result = _groupTravelService.RejectRequest(requestId, participantId);

                var notification = _notificationService.CreateMessageNotification(
                    userId: result.OrganizerId,
                    actorId: participantId,
                    actorUsername: "System",
                    content: $"A participant rejected your group travel request: {result.TourName}",
                    resourceUrl: $"/group-travel/requests/{requestId}"
                );

                await _hubContext.Clients
                    .Group($"user_{result.OrganizerId}")
                    .SendAsync("ReceiveNotification", notification);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{requestId:int}/complete")]
        public async Task<ActionResult<List<TourPurchaseTokenDto>>> CompleteRequest(int requestId)
        {
            try
            {
                var organizerId = GetTouristId();
                var tokens = _groupTravelService.CompleteRequest(requestId, organizerId);

                var request = _groupTravelService.GetMyRequests(organizerId)
                    .FirstOrDefault(r => r.Id == requestId);

                if (request != null)
                {
                    foreach (var participant in request.Participants.Where(p => p.Status == 1))
                    {
                        var notification = _notificationService.CreateMessageNotification(
                            userId: participant.TouristId,
                            actorId: organizerId,
                            actorUsername: request.OrganizerUsername,
                            content: $"Group travel '{request.TourName}' has been completed! You can now start the tour.",
                            resourceUrl: "/tour-execution/purchased-tours"
                        );

                        await _hubContext.Clients
                            .Group($"user_{participant.TouristId}")
                            .SendAsync("ReceiveNotification", notification);
                    }
                }

                return Ok(tokens);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{requestId:int}/cancel")]
        public ActionResult CancelRequest(int requestId)
        {
            try
            {
                var organizerId = GetTouristId();
                _groupTravelService.CancelRequest(requestId, organizerId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
