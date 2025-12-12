using System.Collections.Generic;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tours
{
    [ApiController]
    [Route("api/tours/search")]
    public class TourSearchController : ControllerBase
    {
        private readonly ITourSearchService _service;

        public TourSearchController(ITourSearchService service)
        {
            _service = service;
        }

        
        [HttpPost]
        public ActionResult<List<TourSearchResultDto>> Search([FromBody] TourSearchRequestDto req)
        {
            // 422 — nevalidan unos
            if (req == null || req.RadiusKm <= 0 ||
                req.Lat is < -90 or > 90 ||
                req.Lon is < -180 or > 180)
            {
                return UnprocessableEntity("Invalid lat/lon/radius.");
            }

            // 200 — validan unos
            var result = _service.Search(req);
            return Ok(result);
        }
    }
}