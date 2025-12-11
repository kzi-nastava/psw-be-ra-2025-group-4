using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class ShoppingCartDbRepository : IShoppingCartRepository
    {
        private readonly ToursContext _dbContext;
        private readonly DbSet<ShoppingCart> _dbSet;

        public ShoppingCartDbRepository(ToursContext dbContext)
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
