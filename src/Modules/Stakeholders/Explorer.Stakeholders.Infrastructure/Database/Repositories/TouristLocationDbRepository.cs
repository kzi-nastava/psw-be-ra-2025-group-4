using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class TouristLocationDbRepository : ITouristLocationRepository
    {
        private readonly StakeholdersContext _dbContext;
        private readonly DbSet<TouristLocation> _dbSet;

        public TouristLocationDbRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TouristLocation>();
        }

        public TouristLocation? GetById(long userId)
        {
            return _dbSet.FirstOrDefault(l => l.UserId == userId);
        }

        public TouristLocation Save(TouristLocation entity)
        {
            if (entity.Id == 0)
            {
                _dbSet.Add(entity);
            }
            else
            {
                _dbSet.Update(entity);
            }

            _dbContext.SaveChanges();
            return entity;
        }
    }
}
