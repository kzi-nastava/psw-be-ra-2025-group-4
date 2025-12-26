using System;
using System.Collections.Generic;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Shopping;
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
            var id = User.FindFirst("id")?.Value;
            if (!string.IsNullOrEmpty(id)) return int.Parse(id);

            var pid = User.FindFirst("personId")?.Value;
            if (!string.IsNullOrEmpty(pid)) return int.Parse(pid);

            throw new Exception("No user id found");
        }

        [HttpPost]
        public ActionResult<List<TourPurchaseTokenDto>> Checkout()
        {
            var tokens = _checkoutService.Checkout(GetTouristId());
            return Ok(tokens);
        }
    }
}