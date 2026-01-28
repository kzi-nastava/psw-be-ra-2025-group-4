using System;
using System.Collections.Generic;
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
    [Route("api/tourist/gift-cards")]
    [ApiController]
    public class GiftCardController : ControllerBase
    {
        private readonly IGiftCardService _giftCardService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<MessageHub> _hubContext;

        public GiftCardController(
            IGiftCardService giftCardService,
            INotificationService notificationService,
            IHubContext<MessageHub> hubContext)
        {
            _giftCardService = giftCardService;
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

        
        [HttpGet]
        public ActionResult<List<GiftCardDto>> GetMyGiftCards()
        {
            try
            {
                var touristId = GetTouristId();
                var cards = _giftCardService.GetMyGiftCards(touristId);
                return Ok(cards);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<GiftCardDto>> PurchaseGiftCard([FromBody] PurchaseGiftCardRequestDto request)
        {
            try
            {
                var buyerTouristId = GetTouristId();
                var result = _giftCardService.PurchaseGiftCard(buyerTouristId, request);

                var content = "You have received a gift card! You can use it to purchase Coins Bundles.";
                var notification = _notificationService.CreateMessageNotification(
                    userId: result.RecipientTouristId,
                    actorId: -1,
                    actorUsername: "System",
                    content: content,
                    resourceUrl: "/tourist/coins-bundles"
                );
                await _hubContext.Clients
                    .Group($"user_{result.RecipientTouristId}")
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
    }
}
