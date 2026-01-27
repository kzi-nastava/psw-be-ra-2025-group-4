using System;
using System.Collections.Generic;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist.Payments
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/coins-bundles")]
    [ApiController]
    public class CoinsBundleController : ControllerBase
    {
        private readonly ICoinsBundleService _coinsBundleService;

        public CoinsBundleController(ICoinsBundleService coinsBundleService)
        {
            _coinsBundleService = coinsBundleService;
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
        public ActionResult<List<CoinsBundleDto>> GetAllBundles()
        {
            try
            {
                var bundles = _coinsBundleService.GetAllBundles();
                return Ok(bundles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<CoinsBundleDto> GetBundle(int id)
        {
            try
            {
                var bundle = _coinsBundleService.GetBundle(id);
                return Ok(bundle);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("purchase")]
        public ActionResult<CoinsBundlePurchaseDto> PurchaseBundle([FromBody] PurchaseCoinsBundleRequestDto request)
        {
            try
            {
                var touristId = GetTouristId();
                var purchase = _coinsBundleService.PurchaseBundle(touristId, request);
                return Ok(purchase);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
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

        [HttpGet("purchases")]
        public ActionResult<List<CoinsBundlePurchaseDto>> GetPurchaseHistory()
        {
            try
            {
                var touristId = GetTouristId();
                var purchases = _coinsBundleService.GetPurchaseHistory(touristId);
                return Ok(purchases);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}