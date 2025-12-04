using System.Collections.Generic;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourPurchaseTokenRepository
    {
        TourPurchaseToken Create(TourPurchaseToken token);
        List<TourPurchaseToken> GetByTourist(int touristId);
        bool Exists(int touristId, int tourId);
    }
}