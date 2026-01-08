using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/mystery-offers")]
    [ApiController]
    public class MysteryTourOfferController : ControllerBase
    {
        private readonly IMysteryTourOfferService _service;

        public MysteryTourOfferController(IMysteryTourOfferService service)
        {
            _service = service;
        }

        private int GetTouristId()
        {
            var id = User.FindFirst("id")?.Value;
            if (id != null) return int.Parse(id);
            var pid = User.FindFirst("personId")?.Value;
            return int.Parse(pid ?? throw new Exception("No user id found"));
        }

        [HttpGet("current")]
        public ActionResult<MysteryTourOfferDto> GetOrCreate()
        {
            var dto = _service.GetOrCreate(GetTouristId());
            return Ok(dto);
        }

        [HttpPost("{offerId:guid}/redeem")]
        public IActionResult Redeem(Guid offerId)
        {
            _service.Redeem(offerId, GetTouristId());
            return NoContent();
        }
    }
}
