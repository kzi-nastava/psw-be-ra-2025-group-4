using System.Collections.Generic;
using System.Linq;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TourPurchaseTokenDbRepository : ITourPurchaseTokenRepository
    {
        private readonly ToursContext _dbContext;
        private readonly DbSet<TourPurchaseToken> _dbSet;

        public TourPurchaseTokenDbRepository(ToursContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TourPurchaseToken>();
        }

        public TourPurchaseToken Create(TourPurchaseToken token)
        {
            _dbSet.Add(token);
            _dbContext.SaveChanges();
            return token;
        }

        public List<TourPurchaseToken> GetByTourist(int touristId)
        {
            return _dbSet.AsNoTracking()
                         .Where(t => t.TouristId == touristId)
                         .ToList();
        }

        public bool Exists(int touristId, int tourId)
        {
            return _dbSet.Any(t => t.TouristId == touristId && t.TourId == tourId);
        }
    }
}
