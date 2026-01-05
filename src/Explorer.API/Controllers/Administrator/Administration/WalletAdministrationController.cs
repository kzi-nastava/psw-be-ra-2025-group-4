using System;
using System.Threading.Tasks;
using Explorer.API.Hubs;
using Explorer.Payments.API.Public.Administration;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Explorer.API.Controllers.Administrator.Administration
{
    [Authorize(Policy = "administratorPolicy")]
    [Route("api/administrator/wallet")]
    [ApiController]
    public class WalletAdministrationController : ControllerBase
    {
        private readonly IWalletAdministrationService _walletAdministrationService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<MessageHub> _hubContext;

        public WalletAdministrationController(
            IWalletAdministrationService walletAdministrationService,
            INotificationService notificationService,
            IHubContext<MessageHub> hubContext)
        {
            _walletAdministrationService = walletAdministrationService;
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        [HttpPost("{touristId:int}/add-balance")]
        public async Task<IActionResult> AddBalance(int touristId, [FromBody] AddBalanceDto dto)
        {
            try
            {
                _walletAdministrationService.AddBalance(touristId, dto.Amount);
                
                var content = $"You received {dto.Amount} Adventure Coins (AC) in your wallet.";
                var notification = _notificationService.CreateMessageNotification(
                    userId: touristId,
                    actorId: 0,
                    actorUsername: "System",
                    content: content,
                    resourceUrl: "/tourist/wallet"
                );
                
                await _hubContext.Clients
                    .Group($"user_{touristId}")
                    .SendAsync("ReceiveNotification", notification);
                
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class AddBalanceDto
    {
        public decimal Amount { get; set; }
    }
}

