using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist
{
    public interface IMysteryTourOfferService
    {
        MysteryTourOfferDto GetOrCreate(int touristId);
        RedeemResultDto Redeem(Guid offerId, int touristId);
    }
}
