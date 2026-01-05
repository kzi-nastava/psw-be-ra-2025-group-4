using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class UserDiscoveryService : IUserDiscoveryService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IFollowRepository _followRepository;

        public UserDiscoveryService(
            IUserRepository userRepository,
            IUserProfileRepository userProfileRepository,
            IFollowRepository followRepository)
        {
            _userRepository = userRepository;
            _userProfileRepository = userProfileRepository;
            _followRepository = followRepository;
        }

        public List<UserDiscoveryDto> Search(string query, long currentUserId)
        {
            var users = _userRepository.SearchByUsername(query)
                .Where(u =>
                    u.Id != currentUserId &&
                    u.Role != UserRole.Administrator &&
                    !_followRepository.Exists(currentUserId, u.Id)
                );

            var result = new List<UserDiscoveryDto>();

            foreach (var user in users)
            {
                var profile = _userProfileRepository.GetByUserId(user.Id);

                result.Add(new UserDiscoveryDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    ProfileImageUrl = profile.ProfileImageUrl
                });
            }

            return result;
        }
    }
}
