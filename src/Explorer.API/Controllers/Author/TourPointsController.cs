using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/tours")]
    [ApiController]
    public class TourPointsController : ControllerBase
    {
        private readonly ITourPointService _service;
        private readonly IWebHostEnvironment _env;

        public TourPointsController(ITourPointService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        private int AuthorId()
        {
            var id = User.FindFirst("id")?.Value ?? User.FindFirst("personId")?.Value;
            return int.Parse(id ?? throw new Exception("No user id"));
        }

        private string? SaveImageFromBase64(string? base64)
        {
            if (string.IsNullOrWhiteSpace(base64)) return null;

            var commaIndex = base64.IndexOf(',');
            if (commaIndex >= 0)
            {
                base64 = base64[(commaIndex + 1)..];
            }

            var bytes = Convert.FromBase64String(base64);

            var folder = Path.Combine(_env.WebRootPath, "TourPointsImages");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}.jpg"; 
            var fullPath = Path.Combine(folder, fileName);

            System.IO.File.WriteAllBytes(fullPath, bytes);

            return fileName;
        }


        [HttpGet("{tourId:int}/points")]
        public ActionResult<List<TourPointDto>> GetPoints(int tourId)
        {
            return Ok(_service.GetByTour(tourId, AuthorId()));
        }

        [HttpPost("{tourId:int}/points")]
        public ActionResult<TourPointDto> CreatePoint(int tourId, [FromBody] TourPointDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.ImageBase64))
            {
                dto.ImageFileName = SaveImageFromBase64(dto.ImageBase64);
                dto.ImageBase64 = null;
            }

            var created = _service.Create(tourId, dto, AuthorId());
            return Created(string.Empty, created);
        }

        [HttpPut("points/{pointId:int}")]
        public ActionResult<TourPointDto> UpdatePoint(int pointId, [FromBody] TourPointDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.ImageBase64))
            {
                dto.ImageFileName = SaveImageFromBase64(dto.ImageBase64);
                dto.ImageBase64 = null;
            }

            return Ok(_service.Update(pointId, dto, AuthorId()));
        }

        [HttpDelete("points/{pointId:int}")]
        public IActionResult DeletePoint(int pointId)
        {
            _service.Delete(pointId, AuthorId());
            return NoContent();
        }
    }
}
