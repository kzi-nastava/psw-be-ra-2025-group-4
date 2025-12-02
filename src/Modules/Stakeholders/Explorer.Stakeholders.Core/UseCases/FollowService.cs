using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class FollowService : IFollowService
    {
        private readonly IFollowRepository _followRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public FollowService(IFollowRepository followRepository, IUserRepository userRepository, IMapper mapper)
        {
            _followRepository = followRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public void Follow(long followerId, long followedId)
        {
            var follower = _userRepository.Get(followerId);
            var followed = _userRepository.Get(followedId);

            if (follower == null)
                throw new ArgumentException($"Follower with id '{followerId}' not found.");

            if (followed == null)
                throw new ArgumentException($"User with id '{followedId}' not found.");

            if (follower.Role == UserRole.Administrator || followed.Role == UserRole.Administrator)
                throw new InvalidOperationException("Administrators cannot follow or be followed.");

            if (_followRepository.Exists(followerId, followedId)) return;

            var follow = new Follow(followerId, followedId);
            _followRepository.Create(follow);
        }

        public void Unfollow(long followerId, long followedId)
        {
            _followRepository.Delete(followerId, followedId);
        }

        public List<UserAccountDto> GetFollowers(long userId)
        {
            var followersIds = _followRepository.GetFollowerIdsForUser(userId);
            var users = followersIds.Select(id => _userRepository.Get(id))
                                    .Where(u => u != null && u.Role != UserRole.Administrator)
                                    .ToList();
            return _mapper.Map<List<UserAccountDto>>(users);
        }

        public List<UserAccountDto> GetFollowing(long userId)
        {
            var followingIds = _followRepository.GetFollowingIdsForUser(userId);
            var users = followingIds.Select(id => _userRepository.Get(id))
                                    .Where(u => u != null && u.Role != UserRole.Administrator)
                                    .ToList();
            return _mapper.Map<List<UserAccountDto>>(users);
        }
    }
}
