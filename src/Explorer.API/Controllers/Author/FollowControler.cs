using Explorer.API.Hubs;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Explorer.API.Controllers.Stakeholders
{
    [Route("api/follow")]
    [ApiController]
    [Authorize]
    public class FollowController : ControllerBase
    {
        private readonly IFollowService _followService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly IUserRepository _userRepository;

        public FollowController(IFollowService followService,
            INotificationService notificationService,
            IHubContext<MessageHub> hubContext,
            IUserRepository userRepository)
        {
            _followService = followService;
            _notificationService = notificationService;
            _hubContext = hubContext;
            _userRepository = userRepository;
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> FollowUser(long userId)
        {
            long followerId = long.Parse(User.FindFirst("id").Value);
            if (User.IsInRole("Administrator"))
                return Forbid("Administrator cannot follow users.");

            _followService.Follow(followerId, userId);

            var follower = _userRepository.Get(followerId);
            var followerUsername = follower?.Username ?? "Unknown";

            var resourceUrl = $"/followers/{userId}";

            var notif = _notificationService.CreateFollowNotification(
                userId: userId,
                actorId: followerId,
                actorUsername: followerUsername,
                resourceUrl: resourceUrl
            );

            await _hubContext.Clients
                .Group($"user_{userId}")
                .SendAsync("ReceiveNotification", notif);
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
