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
            var existing = _dbSet.FirstOrDefault(l => l.UserId == entity.UserId);

            if (existing == null)
            {
                _dbSet.Add(entity);
                _dbContext.SaveChanges();
                return entity;
            }
            else
            {
                existing.UpdateLocation(entity.Latitude, entity.Longitude);
                _dbContext.SaveChanges();
                return existing;
            }
        }

    }
}
