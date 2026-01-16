using System.Collections.Generic;
using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public.Tourist
{
    public interface IBundlePurchaseService
    {
        List<TourPurchaseTokenDto> PurchaseBundle(int touristId, int bundleId);
    }
}

