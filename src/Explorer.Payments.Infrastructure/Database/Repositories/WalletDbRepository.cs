using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class WalletDbRepository : IWalletRepository
    {
        private readonly PaymentsContext _dbContext;
        private readonly DbSet<Wallet> _dbSet;

        public WalletDbRepository(PaymentsContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<Wallet>();
        }

        public Wallet? GetByTouristId(int touristId)
        {
            return _dbSet.FirstOrDefault(w => w.TouristId == touristId);
        }

        public Wallet Create(Wallet wallet)
        {
            _dbSet.Add(wallet);
            _dbContext.SaveChanges();
            return wallet;
        }

        public Wallet Update(Wallet wallet)
        {
            _dbSet.Update(wallet);
            _dbContext.SaveChanges();
            return wallet;
        }

        public bool Exists(int touristId)
        {
            return _dbSet.Any(w => w.TouristId == touristId);
        }
    }
}

