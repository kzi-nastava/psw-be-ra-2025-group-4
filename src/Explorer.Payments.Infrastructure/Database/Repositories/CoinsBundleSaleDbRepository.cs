using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class CoinsBundleSaleDbRepository : ICoinsBundleSaleRepository
    {
        private readonly PaymentsContext _dbContext;
        private readonly DbSet<CoinsBundleSale> _dbSet;

        public CoinsBundleSaleDbRepository(PaymentsContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<CoinsBundleSale>();
        }

        public CoinsBundleSale? GetActiveSaleForBundle(int coinsBundleId)
        {
            var now = DateTime.UtcNow;
            return _dbSet
                .Where(s => s.CoinsBundleId == coinsBundleId
                    && s.IsActive
                    && s.StartDate <= now
                    && s.EndDate >= now)
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefault();
        }

        public List<CoinsBundleSale> GetAllActiveSales()
        {
            var now = DateTime.UtcNow;
            return _dbSet
                .Where(s => s.IsActive && s.StartDate <= now && s.EndDate >= now)
                .ToList();
        }

        public List<CoinsBundleSale> GetAll()
        {
            return _dbSet.ToList();
        }

        public CoinsBundleSale? Get(int id)
        {
            return _dbSet.FirstOrDefault(s => s.Id == id);
        }

        public CoinsBundleSale Create(CoinsBundleSale sale)
        {
            _dbSet.Add(sale);
            _dbContext.SaveChanges();
            return sale;
        }

        public CoinsBundleSale Update(CoinsBundleSale sale)
        {
            _dbSet.Update(sale);
            _dbContext.SaveChanges();
            return sale;
        }

        public void Delete(int id)
        {
            var sale = _dbSet.FirstOrDefault(s => s.Id == id);
            if (sale != null)
            {
                _dbSet.Remove(sale);
                _dbContext.SaveChanges();
            }
        }
    }
}