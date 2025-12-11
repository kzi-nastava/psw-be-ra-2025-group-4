using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers
{
    [Authorize]
    [Route("api/profile")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _profileService;

        public UserProfileController(IUserProfileService profileService)
        {
            _profileService = profileService;
        }

        
        private long GetUserIdFromToken()
        {
            var id = User.FindFirst("id")?.Value;
            if (id != null) return long.Parse(id);

            var pid = User.FindFirst("personId")?.Value;
            return long.Parse(pid ?? throw new Exception("No user id found"));
        }

        [HttpGet("mine")]
        public ActionResult<UserProfileDto> GetMyProfile()
        {
            var userId = GetUserIdFromToken();
            var profile = _profileService.Get(userId);
            return Ok(profile);
        }

        [HttpPut("{id:long}")]
        public ActionResult<UserProfileDto> Update(long id, [FromBody] UpdateUserProfileDto UserProfiledto)
        {
            var tokenId = GetUserIdFromToken();

            if (tokenId != id)
                return Forbid("You can update only your own profile.");


            var updated = _profileService.Update(id, UserProfiledto);
            return Ok(updated);
        }

        [HttpGet("{userId:long}")]
        public ActionResult<UserProfileDto> GetByUser(long userId)
        {
            var profile = _profileService.Get(userId);
            return Ok(profile);
        }

    }
}
