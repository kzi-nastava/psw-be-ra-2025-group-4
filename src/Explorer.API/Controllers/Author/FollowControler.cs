using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Stakeholders
{
    [Route("api/follow")]
    [ApiController]
    [Authorize]
    public class FollowController : ControllerBase
    {
        private readonly IFollowService _followService;

        public FollowController(IFollowService followService)
        {
            _followService = followService;
        }

        [HttpPost("{userId}")]
        public IActionResult FollowUser(long userId)
        {
            long followerId = long.Parse(User.FindFirst("id").Value);
            if (User.IsInRole("Administrator"))
                return Forbid("Administrator cannot follow users.");

            _followService.Follow(followerId, userId);
            return Ok();
        }

        [HttpDelete("{userId}")]
        public IActionResult UnfollowUser(long userId)
        {
            long followerId = long.Parse(User.FindFirst("id").Value);
            if (User.IsInRole("Administrator"))
                return Forbid("Administrator cannot unfollow users.");

            _followService.Unfollow(followerId, userId);
            return Ok();
        }

        [HttpGet("followers/{userId}")]
        public IActionResult GetFollowers(long userId)
        {
            return Ok(_followService.GetFollowers(userId));
        }

        [HttpGet("following/{userId}")]
        public IActionResult GetFollowing(long userId)
        {
            return Ok(_followService.GetFollowing(userId));
        }
    }
}
