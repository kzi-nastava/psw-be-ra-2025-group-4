using System.Collections.Generic;
using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public.Shopping
{
    public interface ICheckoutService
    {
        List<TourPurchaseTokenDto> Checkout(int touristId);
    }
}
