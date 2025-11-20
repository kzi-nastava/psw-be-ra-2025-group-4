using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration;

[Authorize(Policy = "administratorPolicy")]
[Route("api/administration/historical-monuments")]
[ApiController]
public class HistoricalMonumentController : ControllerBase
{
    private readonly IHistoricalMonumentService _historicalMonumentService;

    public HistoricalMonumentController(IHistoricalMonumentService historicalMonumentService)
    {
        _historicalMonumentService = historicalMonumentService;
    }

    [HttpGet]
    public ActionResult<PagedResult<HistoricalMonumentDTO>> GetAll(
        [FromQuery] int page,
        [FromQuery] int pageSize)
    {
        return Ok(_historicalMonumentService.GetPaged(page, pageSize));
    }

    [HttpPost]
    public ActionResult<HistoricalMonumentDTO> Create([FromBody] HistoricalMonumentDTO dto)
    {
        return Ok(_historicalMonumentService.Create(dto));
    }

    [HttpPut("{id:long}")]
    public ActionResult<HistoricalMonumentDTO> Update(long id, [FromBody] HistoricalMonumentDTO dto)
    {
        dto.Id = id; 
        return Ok(_historicalMonumentService.Update(dto));
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        _historicalMonumentService.Delete(id);
        return Ok();
    }
}
