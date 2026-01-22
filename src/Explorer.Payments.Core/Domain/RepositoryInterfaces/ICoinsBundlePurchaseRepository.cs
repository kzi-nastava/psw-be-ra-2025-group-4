using System.Collections.Generic;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface ICoinsBundlePurchaseRepository
    {
        CoinsBundlePurchase Create(CoinsBundlePurchase purchase);
        List<CoinsBundlePurchase> GetByTouristId(int touristId);
        List<CoinsBundlePurchase> GetAll(); 
    }
}