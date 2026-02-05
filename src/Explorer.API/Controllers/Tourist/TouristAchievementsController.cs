using Microsoft.AspNetCore.Mvc;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.API.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace Explorer.API.Controllers.Tourist
{
    [ApiController]
    [Authorize(Policy = "touristPolicy")]
    [Route("api/users/{userId}/achievements")]
    public class UserAchievementsController : ControllerBase
    {
        private readonly IUserAchievementService _achievementService;

        public UserAchievementsController(IUserAchievementService achievementService)
        {
            _achievementService = achievementService;
        }

        [HttpGet]
        public ActionResult<List<UserAchievementDto>> GetUserAchievements(long userId)
        {
            var achievements = _achievementService.GetUserAchievements(userId);
            return Ok(achievements);
        }
    }
}
