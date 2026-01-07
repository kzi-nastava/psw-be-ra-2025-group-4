using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers
{
    [Route("api/users/discovery")]
    [ApiController]
    [Authorize]
    public class UserDiscoveryController : ControllerBase
    {
        private readonly IUserDiscoveryService _service;

        public UserDiscoveryController(IUserDiscoveryService service)
        {
            _service = service;
        }

        [HttpGet("search")]
        public IActionResult Search([FromQuery] string search)
        {
            var currentUserId = long.Parse(User.FindFirst("id")!.Value);
          
            return Ok(_service.Search(search, currentUserId));
        }
        [HttpGet]
        public IActionResult Discover([FromQuery] string? search)
        {
            var currentUserId = long.Parse(User.FindFirst("id")!.Value);

            if (string.IsNullOrWhiteSpace(search))
            {
                return Ok(_service.GetAll(currentUserId));
            }

            return Ok(_service.Search(search, currentUserId));
        }
    }
}
