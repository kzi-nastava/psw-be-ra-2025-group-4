using System;
using System.Collections.Generic;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Shopping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/checkout")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        private int GetTouristId()
        {
            var pid = User.FindFirst("personId")?.Value ?? User.FindFirst("id")?.Value;
            if (pid == null) throw new Exception("No person id found in token.");
            return int.Parse(pid);
        }

        /// <summary>
        /// Checkout – kreira token(e) za stavke u korpi i prazni korpu.
        /// </summary>
        [HttpPost]
        public ActionResult<List<TourPurchaseTokenDto>> Checkout()
        {
            var tokens = _checkoutService.Checkout(GetTouristId());
            return Ok(tokens);
        }
    }
}