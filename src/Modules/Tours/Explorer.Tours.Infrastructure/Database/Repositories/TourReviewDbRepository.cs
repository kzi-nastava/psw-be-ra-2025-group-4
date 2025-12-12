using System.Collections.Generic;
using System.Linq;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TourReviewDbRepository : ITourReviewRepository
    {
        protected readonly ToursContext DbContext;
        private readonly DbSet<TourReview> _dbSet;

        public TourReviewDbRepository(ToursContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<TourReview>();
        }

        public TourReview Create(TourReview tourReview)
        {
            _dbSet.Add(tourReview);
            DbContext.SaveChanges();
            return tourReview;
        }

        public TourReview Update(TourReview tourReview)
        {
            try
            {
                DbContext.Update(tourReview);
                DbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new NotFoundException(e.Message);
            }
            return tourReview;
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            _dbSet.Remove(entity);
            DbContext.SaveChanges();
        }

        public TourReview GetById(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null) throw new NotFoundException("TourReview not found: " + id);
            return entity;
        }

        public IEnumerable<TourReview> GetByTourist(int touristId)
        {
            return _dbSet.Where(tr => tr.TouristId == touristId).ToList();
        }

        public IEnumerable<TourReview> GetByTour(int tourId)
        {
            return _dbSet.Where(tr => tr.TourId == tourId).ToList();
        }

        public TourReview GetByTouristAndTour(int touristId, int tourId)
        {
            return _dbSet.FirstOrDefault(tr => tr.TouristId == touristId && tr.TourId == tourId);
        }
    }
}