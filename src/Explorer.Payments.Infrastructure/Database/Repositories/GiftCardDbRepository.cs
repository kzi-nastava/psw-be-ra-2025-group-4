using System.Collections.Generic;
using System.Linq;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class GiftCardDbRepository : IGiftCardRepository
    {
        private readonly PaymentsContext _dbContext;
        private readonly DbSet<GiftCard> _dbSet;

        public GiftCardDbRepository(PaymentsContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<GiftCard>();
        }

        public GiftCard? GetByCode(string code)
        {
            return _dbSet.FirstOrDefault(g => g.Code == code);
        }

        public List<GiftCard> GetByRecipientTouristId(int touristId)
        {
            return _dbSet
                .Where(g => g.RecipientTouristId == touristId && g.Balance > 0)
                .OrderByDescending(g => g.PurchasedAt)
                .ToList();
        }

        public GiftCard Create(GiftCard giftCard)
        {
            _dbSet.Add(giftCard);
            _dbContext.SaveChanges();
            return giftCard;
        }

        public GiftCard Update(GiftCard giftCard)
        {
            _dbContext.Entry(giftCard).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return giftCard;
        }

        public bool ExistsCode(string code)
        {
            return _dbSet.Any(g => g.Code == code);
        }
    }
}
