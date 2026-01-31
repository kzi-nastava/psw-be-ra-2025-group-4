using System;
using System.Linq;
using Explorer.Payments.Core.Domain;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface IAffiliateRedemptionRepository
    {
        AffiliateRedemption Create(AffiliateRedemption redemption);
        IQueryable<AffiliateRedemption> Query();
    }
}
