using System;
using System.Collections.Generic;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist.Payments
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
            var pid = User.FindFirst("personId")?.Value;
            if (!string.IsNullOrWhiteSpace(pid)) return int.Parse(pid);

            var id = User.FindFirst("id")?.Value;
            if (!string.IsNullOrWhiteSpace(id)) return int.Parse(id);

            throw new Exception("No user id found");
        }

        [HttpPost]
        public ActionResult<List<TourPurchaseTokenDto>> Checkout()
        {
            try
            {
                var tokens = _checkoutService.Checkout(GetTouristId());
                return Ok(tokens);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}