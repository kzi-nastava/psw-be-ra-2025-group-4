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
    [Route("api/tourist/checkout")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<MessageHub> _hubContext;

        public CheckoutController(
            ICheckoutService checkoutService,
            INotificationService notificationService,
            IHubContext<MessageHub> hubContext)
        {
            _checkoutService = checkoutService;
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
        public ActionResult<List<TourPurchaseTokenDto>> GetPurchaseTokens()
        {
            try
            {
                var touristId = GetTouristId();
                var tokens = _checkoutService.GetPurchaseTokens(touristId);
                return Ok(tokens);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<List<TourPurchaseTokenDto>>> Checkout()
        {
            try
            {
                var touristId = GetTouristId();
                var tokens = _checkoutService.Checkout(touristId);
                
                var tourCount = tokens.Count;
                var content = tourCount == 1 
                    ? "A new tour has been added to your collection!" 
                    : $"{tourCount} new tours have been added to your collection!";
                
                var notification = _notificationService.CreateMessageNotification(
                    userId: touristId,
                    actorId: -1,
                    actorUsername: "System",
                    content: content,
                    resourceUrl: "/tour-execution/all-tours"
                );
                
                await _hubContext.Clients
                    .Group($"user_{touristId}")
                    .SendAsync("ReceiveNotification", notification);
                
                return Ok(tokens);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}