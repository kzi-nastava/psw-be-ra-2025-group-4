using System;
using System.Collections.Generic;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Payments
{
    [Authorize(Policy = "administratorPolicy")]
    [Route("api/administration/coins-bundle-sales")]
    [ApiController]
    public class CoinsBundleSaleController : ControllerBase
    {
        private readonly ICoinsBundleSaleService _saleService;

        public CoinsBundleSaleController(ICoinsBundleSaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpPost]
        public ActionResult<CoinsBundleSaleDto> CreateSale([FromBody] CoinsBundleSaleDto saleDto)
        {
            try
            {
                var sale = _saleService.CreateSale(saleDto);
                return Ok(sale);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<List<CoinsBundleSaleDto>> GetAllSales()
        {
            try
            {
                var sales = _saleService.GetAllSales();
                return Ok(sales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<CoinsBundleSaleDto> GetSale(int id)
        {
            try
            {
                var sale = _saleService.GetSale(id);
                return Ok(sale);
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

        [HttpPut("{id}/deactivate")]
        public ActionResult DeactivateSale(int id)
        {
            try
            {
                _saleService.DeactivateSale(id);
                return Ok();
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

        [HttpDelete("{id}")]
        public ActionResult DeleteSale(int id)
        {
            try
            {
                _saleService.DeleteSale(id);
                return NoContent();
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

        [HttpGet("purchases")]
        public ActionResult<List<CoinsBundlePurchaseDto>> GetAllPurchases()
        {
            try
            {
                var purchases = _saleService.GetAllPurchases();
                return Ok(purchases);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}