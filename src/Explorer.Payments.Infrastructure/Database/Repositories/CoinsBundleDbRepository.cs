using System.Collections.Generic;
using System.Linq;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class CoinsBundleDbRepository : ICoinsBundleRepository
    {
        private readonly PaymentsContext _dbContext;
        private readonly DbSet<CoinsBundle> _dbSet;

        public CoinsBundleDbRepository(PaymentsContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<CoinsBundle>();
        }

        public List<CoinsBundle> GetAll()
        {
            return _dbSet.OrderBy(b => b.DisplayOrder).ToList();
        }

        public CoinsBundle? Get(int id)
        {
            return _dbSet.FirstOrDefault(b => b.Id == id);
        }

        public CoinsBundle Create(CoinsBundle bundle)
        {
            _dbSet.Add(bundle);
            _dbContext.SaveChanges();
            return bundle;
        }

        public CoinsBundle Update(CoinsBundle bundle)
        {
            _dbSet.Update(bundle);
            _dbContext.SaveChanges();
            return bundle;
        }
    }
}