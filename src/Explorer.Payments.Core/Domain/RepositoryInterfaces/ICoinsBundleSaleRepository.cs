using System.Collections.Generic;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface ICoinsBundleSaleRepository
    {
        CoinsBundleSale? GetActiveSaleForBundle(int coinsBundleId);
        List<CoinsBundleSale> GetAllActiveSales();
        List<CoinsBundleSale> GetAll();
        CoinsBundleSale? Get(int id);
        CoinsBundleSale Create(CoinsBundleSale sale);
        CoinsBundleSale Update(CoinsBundleSale sale);
        void Delete(int id);
    }
}