using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
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

            public TourReview GetById(long id)
            {
                var entity = _dbSet.FirstOrDefault(r => r.Id == id);
                if (entity == null)
                    throw new KeyNotFoundException("TourReview not found: " + id);
                return entity;
            }

            public TourReview Update(TourReview review)
            {
                DbContext.Update(review);
                DbContext.SaveChanges();
                return review;
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