using System;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist.Payments
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/shopping-cart")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
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
        public ActionResult<ShoppingCartDto> Get()
        {
            var result = _shoppingCartService.GetForTourist(GetTouristId());
            return Ok(result);
        }


        [HttpPost("{tourId:int}")]
        public ActionResult<ShoppingCartDto> AddToCart(int tourId)
        {
            var result = _shoppingCartService.AddToCart(GetTouristId(), tourId);
            return Ok(result);
        }



        [HttpDelete("{tourId:int}")]
        public ActionResult<ShoppingCartDto> RemoveFromCart(int tourId)
        {
            var result = _shoppingCartService.RemoveFromCart(GetTouristId(), tourId);
            return Ok(result);
        }



    }
}
