using System.Collections.Generic;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface IGiftCardRepository
    {
        GiftCard? GetByCode(string code);
        List<GiftCard> GetByRecipientTouristId(int touristId);
        GiftCard Create(GiftCard giftCard);
        GiftCard Update(GiftCard giftCard);
        bool ExistsCode(string code);
    }
}
