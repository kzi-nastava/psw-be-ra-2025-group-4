using System.Linq;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class UserProfileDbRepository : IUserProfileRepository
    {
        protected readonly StakeholdersContext DbContext;
        private readonly DbSet<UserProfile> _dbSet;

        public UserProfileDbRepository(StakeholdersContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<UserProfile>();
        }

        public UserProfile GetByUserId(long userId)
        {
            var entity = _dbSet.FirstOrDefault(p => p.UserId == userId);

            if (entity == null)
                throw new NotFoundException("User profile not found: " + userId);

            return entity;
        }

        public UserProfile Update(UserProfile profile)
        {
            try
            {
                DbContext.Update(profile);
                DbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new NotFoundException(e.Message);
            }

            return profile;
        }

       
    }
}
