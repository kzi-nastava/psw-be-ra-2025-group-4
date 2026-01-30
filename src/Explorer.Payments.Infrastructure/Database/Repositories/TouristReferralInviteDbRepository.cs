using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class TouristReferralInviteDbRepository : ITouristReferralInviteRepository
    {
        private readonly PaymentsContext _dbContext;
        private readonly DbSet<TouristReferralInvite> _dbSet;

        public TouristReferralInviteDbRepository(PaymentsContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TouristReferralInvite>();
        }

        public TouristReferralInvite? GetByCode(string code)
        {
            return _dbSet.FirstOrDefault(i => i.Code == code);
        }

        public bool CodeExists(string code)
        {
            return _dbSet.Any(i => i.Code == code);
        }

        public TouristReferralInvite Create(TouristReferralInvite invite)
        {
            _dbSet.Add(invite);
            _dbContext.SaveChanges();
            return invite;
        }

        public TouristReferralInvite Update(TouristReferralInvite invite)
        {
            _dbSet.Update(invite);
            _dbContext.SaveChanges();
            return invite;
        }
    }
}
