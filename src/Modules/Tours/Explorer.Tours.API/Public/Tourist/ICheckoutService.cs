using System.Collections.Generic;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Shopping
{
    public interface ICheckoutService
    {
        List<TourPurchaseTokenDto> Checkout(int touristId);
    }
}
