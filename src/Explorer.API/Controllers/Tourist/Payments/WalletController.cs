using System;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist.Payments
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/wallet")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
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
        public ActionResult<WalletDto> GetWallet()
        {
            var wallet = _walletService.GetWallet(GetTouristId());
            return Ok(wallet);
        }
    }
}

