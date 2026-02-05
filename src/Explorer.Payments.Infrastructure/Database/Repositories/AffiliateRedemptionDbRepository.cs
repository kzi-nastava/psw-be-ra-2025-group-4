using System.Linq;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class AffiliateRedemptionDbRepository : IAffiliateRedemptionRepository
    {
        private readonly PaymentsContext _db;
        private readonly DbSet<AffiliateRedemption> _set;

        public AffiliateRedemptionDbRepository(PaymentsContext db)
        {
            _db = db;
            _set = db.Set<AffiliateRedemption>();
        }

        public AffiliateRedemption Create(AffiliateRedemption redemption)
        {
            _set.Add(redemption);
            _db.SaveChanges();
            return redemption;
        }

        public IQueryable<AffiliateRedemption> Query()
        {
            return _set.AsNoTracking();
        }
    }
}
