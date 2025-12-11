using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IFollowService
    {
        void Follow(long followerId, long followedId);
        void Unfollow(long followerId, long followedId);
        List<UserAccountDto> GetFollowers(long userId);
        List<UserAccountDto> GetFollowing(long userId);
    }
}

