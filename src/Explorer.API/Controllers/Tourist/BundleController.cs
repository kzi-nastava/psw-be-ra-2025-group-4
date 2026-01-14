using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Explorer.API.Hubs;
using System;
using System.Threading.Tasks;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/bundles")]
    [ApiController]
    public class BundleController : ControllerBase
    {
        private readonly ITouristBundleService _bundleService;
        private readonly IBundlePurchaseService _purchaseService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly IPaymentRecordRepository _paymentRecordRepository;

        public BundleController(
            ITouristBundleService bundleService,
            IBundlePurchaseService purchaseService,
            INotificationService notificationService,
            IHubContext<MessageHub> hubContext,
            IPaymentRecordRepository paymentRecordRepository)
        {
            _bundleService = bundleService;
            _purchaseService = purchaseService;
            _notificationService = notificationService;
            _hubContext = hubContext;
            _paymentRecordRepository = paymentRecordRepository;
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
        public ActionResult<PagedResult<BundleDto>> GetPublished([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = _bundleService.GetPublished(page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public ActionResult<BundleDto> GetById(int id)
        {
            try
            {
                var bundle = _bundleService.GetById(id);
                return Ok(bundle);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("purchased/{bundleId:int}")]
        public ActionResult<bool> IsPurchased(int bundleId)
        {
            var touristId = GetTouristId();
            var isPurchased = _paymentRecordRepository.ExistsForBundle(touristId, bundleId);
            return Ok(isPurchased);
        }

        [HttpPost("{id:int}/purchase")]
        public async Task<ActionResult<List<TourPurchaseTokenDto>>> Purchase(int id)
        {
            try
            {
                var touristId = GetTouristId();
                var tokens = _purchaseService.PurchaseBundle(touristId, id);
                
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
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
