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

        public Dictionary<int, ReviewStats> GetStatsForTours(IEnumerable<int> tourIds)
        {
            var ids = (tourIds ?? Enumerable.Empty<int>()).Distinct().ToList();
            if (!ids.Any()) return new Dictionary<int, ReviewStats>();

            var data = _dbSet.AsNoTracking()
                .Where(x => ids.Contains(x.TourId))
                .GroupBy(x => x.TourId)
                .Select(g => new
                {
                    TourId = g.Key,
                    Count = g.Count(),
                    Avg = g.Average(x => (double)x.Rating)
                })
                .ToList();

            return data.ToDictionary(
                x => x.TourId,
                x => new ReviewStats
                {
                    Count = x.Count,
                    AvgRating = x.Count == 0 ? 0.0 : x.Avg
                });
        }

        public List<TourReview> GetLatestForTour(int tourId, int take)
        {
            if (take <= 0) take = 5;

            return _dbSet.AsNoTracking()
                .Where(x => x.TourId == tourId)
                .OrderByDescending(x => x.CreatedAt)
                .Take(take)
                .ToList();
        }

        public List<(DateTime Date, double Avg, int Count)> GetDailyReviewStats(int tourId, DateTime from, DateTime to)
        {
            return _dbSet.AsNoTracking()
                .Where(x => x.TourId == tourId && x.CreatedAt >= from && x.CreatedAt <= to)
                .GroupBy(x => x.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count(),
                    Avg = g.Average(x => (double)x.Rating)
                })
                .OrderBy(x => x.Date)
                .ToList()
                .Select(x => (x.Date, x.Avg, x.Count))
                .ToList();
        }
    }
}