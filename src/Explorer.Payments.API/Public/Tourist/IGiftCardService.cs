using System.Collections.Generic;
using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public.Tourist
{
    public interface IGiftCardService
    {
        GiftCardDto PurchaseGiftCard(int buyerTouristId, PurchaseGiftCardRequestDto request);
        List<GiftCardDto> GetMyGiftCards(int touristId);
    }
}
