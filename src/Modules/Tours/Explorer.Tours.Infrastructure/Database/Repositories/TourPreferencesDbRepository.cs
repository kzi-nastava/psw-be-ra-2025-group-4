using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Explorer.Tours.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database.Repositories;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TourPreferencesDbRepository : ITourPreferencesRepository
    {
        protected readonly ToursContext DbContext;
        private readonly DbSet<TourPreferences> _dbSet;

        public TourPreferencesDbRepository(ToursContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<TourPreferences>();
        }

        public TourPreferences? GetByTouristId(int touristId)
        {
            return _dbSet.FirstOrDefault(p => p.TouristId == touristId);
        }

        public TourPreferences Create(TourPreferences preferences)
        {
            _dbSet.Add(preferences);
            DbContext.SaveChanges();
            return preferences;
        }

        public TourPreferences Update(TourPreferences preferences)
        {
            try
            {
                DbContext.Update(preferences);
                DbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
               
                throw new NotFoundException(e.Message);
            }

            return preferences;
        }

        public void DeleteByTouristId(int touristId)
        {
            var existing = GetByTouristId(touristId);
            if (existing == null) return; 

            _dbSet.Remove(existing);
            DbContext.SaveChanges();
        }
    }
}
