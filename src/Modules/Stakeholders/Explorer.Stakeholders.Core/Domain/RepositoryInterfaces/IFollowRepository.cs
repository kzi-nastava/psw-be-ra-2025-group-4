using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IFollowRepository
    {
        List<long> GetFollowerIdsForUser(long userId);
        List<long> GetFollowingIdsForUser(long userId);
        bool Exists(long followerId, long followedId);
        Follow Create(Follow follow);
        void Delete(long followerId, long followedId);
    }
}

