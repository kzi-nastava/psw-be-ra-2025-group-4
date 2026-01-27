using System.Collections.Generic;
using System.Linq;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class CoinsBundlePurchaseDbRepository : ICoinsBundlePurchaseRepository
    {
        private readonly PaymentsContext _dbContext;
        private readonly DbSet<CoinsBundlePurchase> _dbSet;

        public CoinsBundlePurchaseDbRepository(PaymentsContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<CoinsBundlePurchase>();
        }

        public CoinsBundlePurchase Create(CoinsBundlePurchase purchase)
        {
            _dbSet.Add(purchase);
            _dbContext.SaveChanges();
            return purchase;
        }

        public List<CoinsBundlePurchase> GetByTouristId(int touristId)
        {
            return _dbSet
                .Where(p => p.TouristId == touristId)
                .OrderByDescending(p => p.PurchaseDate)
                .ToList();
        }

        public List<CoinsBundlePurchase> GetAll()
        {
            return _dbSet
                .OrderByDescending(p => p.PurchaseDate)
                .ToList();
        }
    }
}