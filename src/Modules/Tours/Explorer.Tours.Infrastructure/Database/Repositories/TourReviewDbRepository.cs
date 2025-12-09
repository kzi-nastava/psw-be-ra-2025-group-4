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

        public TourReview Create(TourReview review)
        {
            _dbSet.Add(review);
            DbContext.SaveChanges();
            return review;
        }

        public TourReview Update(TourReview review)
        {
            try
            {
                DbContext.Update(review);
                DbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new NotFoundException(e.Message);
            }
            return review;
        }

        public void Delete(long id)
        {
            var entity = GetById(id);
            _dbSet.Remove(entity);
            DbContext.SaveChanges();
        }

        public TourReview GetById(long id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null) throw new NotFoundException("TourReview not found: " + id);
            return entity;
        }

        public TourReview GetByTouristAndTour(long touristId, int tourId)
        {
            return _dbSet
                .FirstOrDefault(r => r.TouristId == touristId && r.TourId == tourId);
        }

        public bool HasReview(long touristId, int tourId)
        {
            return _dbSet
                .Any(r => r.TouristId == touristId && r.TourId == tourId);
        }
    }
}