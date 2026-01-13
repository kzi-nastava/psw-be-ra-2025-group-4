using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IMysteryTourOfferRepository
    {
        MysteryTourOffer? GetActiveForTourist(int touristId);
        MysteryTourOffer Create(MysteryTourOffer offer);
        MysteryTourOffer Update(MysteryTourOffer offer);
    }
}
