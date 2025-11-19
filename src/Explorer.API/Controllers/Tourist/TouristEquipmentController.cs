using System.Linq;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/equipment")]
[ApiController]

public class TouristEquipmentController : ControllerBase
{
    private readonly ITouristEquipmentService _touristEquipmentService;

    public TouristEquipmentController(ITouristEquipmentService touristEquipmentService)
    {
        _touristEquipmentService = touristEquipmentService;
    }
    private long GetTouristId()
    {
        var idClaim = User.Claims.FirstOrDefault(c => c.Type == "personId")?.Value;
        if (string.IsNullOrWhiteSpace(idClaim))
            throw new UnauthorizedAccessException("Missing personId claim.");

        return long.Parse(idClaim);
    }


    [HttpGet]
    public ActionResult<List<TouristEquipmentDTO>> GetMyEquipment()
    {
        var touristId = GetTouristId();
        var result = _touristEquipmentService.GetForTourist(touristId);
        return Ok(result);
    }

  

    [HttpPost]
    public ActionResult<TouristEquipmentDTO> AddEquipment([FromBody] long equipmentId)
    {
        var touristId = GetTouristId();

        var dto = new TouristEquipmentDTO
        {
            TouristId = touristId,
            EquipmentId = equipmentId
        };

        var created = _touristEquipmentService.Add(dto);
        return Ok(created);
    }

    
    [HttpDelete("{id:long}")]
    public ActionResult RemoveEquipment(long id)
    {
        _touristEquipmentService.Remove(id);
        return Ok();
    }
}