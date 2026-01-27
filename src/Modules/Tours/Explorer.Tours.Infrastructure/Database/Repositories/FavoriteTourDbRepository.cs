using System.Collections.Generic;
using System.Linq;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class FavoriteTourDbRepository : IFavoriteTourRepository
    {
        protected readonly ToursContext DbContext;
        private readonly DbSet<FavoriteTour> _dbSet;

        public FavoriteTourDbRepository(ToursContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<FavoriteTour>();
        }

        public FavoriteTour Create(FavoriteTour favoriteTour)
        {
            _dbSet.Add(favoriteTour);
            DbContext.SaveChanges();
            return favoriteTour;
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            _dbSet.Remove(entity);
            DbContext.SaveChanges();
        }

        public FavoriteTour GetById(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null) throw new NotFoundException("FavoriteTour not found: " + id);
            return entity;
        }

        public FavoriteTour GetByTouristAndTour(int touristId, int tourId)
        {
            return _dbSet.FirstOrDefault(ft => ft.TouristId == touristId && ft.TourId == tourId);
        }

        public IEnumerable<FavoriteTour> GetByTourist(int touristId)
        {
            return _dbSet.Where(ft => ft.TouristId == touristId).OrderByDescending(ft => ft.AddedAt).ToList();
        }

        public bool Exists(int touristId, int tourId)
        {
            return _dbSet.Any(ft => ft.TouristId == touristId && ft.TourId == tourId);
        }
    }
}
