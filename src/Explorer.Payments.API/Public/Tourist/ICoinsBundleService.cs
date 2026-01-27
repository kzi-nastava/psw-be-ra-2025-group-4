using System.Collections.Generic;
using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public.Tourist
{
    public interface ICoinsBundleService
    {
        List<CoinsBundleDto> GetAllBundles();
        CoinsBundleDto GetBundle(int id);
        CoinsBundlePurchaseDto PurchaseBundle(int touristId, PurchaseCoinsBundleRequestDto request);
        List<CoinsBundlePurchaseDto> GetPurchaseHistory(int touristId);
    }
}