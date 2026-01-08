using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class MysteryTourOfferDbRepository : IMysteryTourOfferRepository
    {
        private readonly ToursContext _context; 

        public MysteryTourOfferDbRepository(ToursContext context)
        {
            _context = context;
        }

        public MysteryTourOffer? GetActiveForTourist(int touristId)
        {
            var now = DateTime.UtcNow;

            return _context.MysteryTourOffers
                .AsNoTracking()
                .Where(o => o.TouristId == touristId
                            && !o.Redeemed
                            && o.ExpiresAt > now)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault();
        }

        public MysteryTourOffer Create(MysteryTourOffer offer)
        {
            _context.MysteryTourOffers.Add(offer);
            _context.SaveChanges();
            return offer;
        }

        public MysteryTourOffer Update(MysteryTourOffer offer)
        {
            _context.MysteryTourOffers.Update(offer);
            _context.SaveChanges();
            return offer;
        }
    }
}
