using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;


namespace Explorer.API.Controllers
{
    [Authorize]
    [Route("api/profile")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _profileService;
        private readonly IWebHostEnvironment _env;

        public UserProfileController(IUserProfileService profileService, IWebHostEnvironment env)
        {
            _profileService = profileService;
            _env = env;
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

            if (!string.IsNullOrWhiteSpace(UserProfiledto.ProfileImageBase64))
            {
                UserProfiledto.ProfileImageUrl = SaveImageFromBase64(UserProfiledto.ProfileImageBase64);
                UserProfiledto.ProfileImageBase64 = null;
            }


            var updated = _profileService.Update(id, UserProfiledto);
            return Ok(updated);
        }

        [HttpGet("{userId:long}")]
        public ActionResult<UserProfileDto> GetByUser(long userId)
        {
            var profile = _profileService.Get(userId);
            return Ok(profile);
        }

        private string SaveImageFromBase64(string base64)
        {
            var commaIndex = base64.IndexOf(',');
            if (commaIndex >= 0) base64 = base64[(commaIndex + 1)..];

            var bytes = Convert.FromBase64String(base64);

            var webRoot = _env.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
                webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var folder = Path.Combine(webRoot, "ProfileImages");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}.jpg";
            var fullPath = Path.Combine(folder, fileName);

            System.IO.File.WriteAllBytes(fullPath, bytes);

            return $"/ProfileImages/{fileName}";
        }


    }
}
