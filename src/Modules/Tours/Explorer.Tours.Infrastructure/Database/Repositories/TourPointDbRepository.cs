using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TourPointDbRepository : ITourPointRepository
    {
        private readonly ToursContext _db;
        private readonly DbSet<TourPoint> _dbSet;

        public TourPointDbRepository(ToursContext db)
        {
            _db = db;
            _dbSet = db.Set<TourPoint>();
        }

        public IEnumerable<TourPoint> GetByTour(int tourId)
        {
            return _dbSet.Where(p => p.TourId == tourId)
                         .OrderBy(p => p.Order)
                         .ToList();
        }

        public TourPoint Get(int id)
        {
            var p = _dbSet.Find(id);
            if (p == null) throw new NotFoundException($"Point not found {id}");
            return p;
        }

        public TourPoint Create(TourPoint point)
        {
            _dbSet.Add(point);
            _db.SaveChanges();
            return point;
        }

        public TourPoint Update(TourPoint point)
        {
            _db.Update(point);
            _db.SaveChanges();
            return point;
        }

        public void Delete(int id)
        {
            var p = Get(id);
            _dbSet.Remove(p);
            _db.SaveChanges();
        }
    }
}
