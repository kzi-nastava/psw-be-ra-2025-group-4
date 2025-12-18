using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class FollowDbRepository : IFollowRepository
    {
        private readonly StakeholdersContext _dbContext;
        private readonly DbSet<Follow> _dbSet;

        public FollowDbRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<Follow>();
        }

        public Follow Create(Follow follow)
        {
            _dbSet.Add(follow);
            _dbContext.SaveChanges();
            return follow;
        }

        public bool Exists(long followerId, long followedId)
        {
            return _dbSet.Any(f => f.FollowerId == followerId && f.FollowedId == followedId);
        }

        public void Delete(long followerId, long followedId)
        {
            var entity = _dbSet.FirstOrDefault(f => f.FollowerId == followerId && f.FollowedId == followedId);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                _dbContext.SaveChanges();
            }
        }

        public List<long> GetFollowerIdsForUser(long userId)
        {
            return _dbSet
                .Where(f => f.FollowedId == userId)
                .Select(f => f.FollowerId)
                .ToList();
        }

        public List<long> GetFollowingIdsForUser(long userId)
        {
            return _dbSet
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FollowedId)
                .ToList();
        }
    }
}

