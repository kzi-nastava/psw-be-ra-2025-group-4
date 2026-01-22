using System.Collections.Generic;
using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public.Administration
{
    public interface ICoinsBundleSaleService
    {
        CoinsBundleSaleDto CreateSale(CoinsBundleSaleDto saleDto);
        List<CoinsBundleSaleDto> GetAllSales();
        CoinsBundleSaleDto GetSale(int id);
        void DeactivateSale(int id);
        void DeleteSale(int id);
        List<CoinsBundlePurchaseDto> GetAllPurchases();
    }
}