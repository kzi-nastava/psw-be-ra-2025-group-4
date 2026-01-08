using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class ShoppingCartDbRepository : IShoppingCartRepository
    {
        private readonly PaymentsContext _dbContext;
        private readonly DbSet<ShoppingCart> _dbSet;

        public ShoppingCartDbRepository(PaymentsContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<ShoppingCart>();
        }

        public ShoppingCart? GetByTouristId(int touristId)
        {
            return _dbSet
                .Include(c => c.Items)
                .FirstOrDefault(c => c.TouristId == touristId);
        }

        public ShoppingCart Create(ShoppingCart cart)
        {
            _dbSet.Add(cart);
            _dbContext.SaveChanges();
            return cart;
        }

        public ShoppingCart Update(ShoppingCart cart)
        {
            _dbContext.Update(cart);
            _dbContext.SaveChanges();
            return cart;
        }
    }
}
