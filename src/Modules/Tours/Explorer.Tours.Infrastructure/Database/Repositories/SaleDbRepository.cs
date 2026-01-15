using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class SaleDbRepository : ISaleRepository
    {
        private readonly ToursContext _dbContext;

        public SaleDbRepository(ToursContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Sale Create(Sale sale)
        {
            _dbContext.Sales.Add(sale);
            _dbContext.SaveChanges();
            return sale;
        }

        public Sale Update(Sale sale)
        {
            _dbContext.Sales.Update(sale);
            _dbContext.SaveChanges();
            return sale;
        }

        public void Delete(int id)
        {
            var sale = GetById(id);
            _dbContext.Sales.Remove(sale);
            _dbContext.SaveChanges();
        }

        public Sale GetById(int id)
        {
            var sale = _dbContext.Sales.FirstOrDefault(s => s.Id == id);
            if (sale == null)
                throw new NotFoundException($"Sale not found: {id}");
            return sale;
        }

        public List<Sale> GetByAuthor(int authorId)
        {
            return _dbContext.Sales
                .Where(s => s.AuthorId == authorId)
                .OrderByDescending(s => s.StartDate)
                .ToList();
        }

        public List<Sale> GetActiveSales()
        {
            var now = System.DateTime.UtcNow;
            return _dbContext.Sales
                .Where(s => s.IsActive && s.StartDate <= now && s.EndDate >= now)
                .ToList();
        }

        public List<Sale> GetActiveSalesForTour(int tourId)
        {
            var now = System.DateTime.UtcNow;
            return _dbContext.Sales
                .Where(s => s.IsActive && s.StartDate <= now && s.EndDate >= now && s.TourIds.Contains(tourId))
                .ToList();
        }

        public List<Sale> GetActiveSalesForTours(List<int> tourIds)
        {
            var now = System.DateTime.UtcNow;
            return _dbContext.Sales
                .Where(s => s.IsActive && s.StartDate <= now && s.EndDate >= now)
                .Where(s => s.TourIds.Any(tourId => tourIds.Contains(tourId)))
                .ToList();
        }
    }
}

