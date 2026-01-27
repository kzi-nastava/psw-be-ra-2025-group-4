using System.Collections.Generic;
using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public.Tourist
{
    public interface ICheckoutService
    {
        List<TourPurchaseTokenDto> Checkout(int touristId);
        List<TourPurchaseTokenDto> GetPurchaseTokens(int touristId);
    }
}
