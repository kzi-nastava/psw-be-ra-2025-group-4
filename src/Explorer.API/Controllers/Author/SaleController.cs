using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/sales")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly ITourSaleService _saleService;

        public SaleController(ITourSaleService saleService)
        {
            _saleService = saleService;
        }

        private int GetAuthorId()
        {
            var id = User.FindFirst("id")?.Value;

            if (id != null) return int.Parse(id);

            var pid = User.FindFirst("personId")?.Value;

            return int.Parse(pid ?? throw new Exception("No user id found"));
        }

        [HttpGet]
        public ActionResult<List<SaleDto>> GetAll()
        {
            return Ok(_saleService.GetByAuthor(GetAuthorId()));
        }

        [HttpGet("{id:int}")]
        public ActionResult<SaleDto> GetById(int id)
        {
            var sale = _saleService.GetById(id, GetAuthorId());
            return Ok(sale);
        }

        [HttpPost]
        public ActionResult<SaleDto> Create([FromBody] SaleCreateDto dto)
        {
            var created = _saleService.Create(dto, GetAuthorId());
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public ActionResult<SaleDto> Update(int id, [FromBody] SaleUpdateDto dto)
        {
            var updated = _saleService.Update(id, dto, GetAuthorId());
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            _saleService.Delete(id, GetAuthorId());
            return NoContent();
        }
    }
}

