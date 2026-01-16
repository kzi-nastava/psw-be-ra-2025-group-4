using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class SocialEncounterParticipantDbRepository : ISocialEncounterParticipantRepository
    {
        protected readonly EncountersContext DbContext;
        private readonly DbSet<SocialEncounterParticipant> _dbSet;

        public SocialEncounterParticipantDbRepository(EncountersContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<SocialEncounterParticipant>();
        }

        public void Add(SocialEncounterParticipant participant)
        {
            _dbSet.Add(participant);
            DbContext.SaveChanges();
        }

        public SocialEncounterParticipant? Get(long encounterId, long touristId)
        {
            return _dbSet
                .FirstOrDefault(p => p.SocialEncounterId == encounterId && p.TouristId == touristId);
        }

        public void Update(SocialEncounterParticipant participant)
        {
            _dbSet.Update(participant);
            DbContext.SaveChanges();
        }

        public void Remove(SocialEncounterParticipant participant)
        {
            _dbSet.Remove(participant);
            DbContext.SaveChanges();
        }

        public void RemoveStale(long encounterId, TimeSpan threshold, DateTime referenceTime)
        {
            var cutoff = referenceTime - threshold - TimeSpan.FromMilliseconds(100);
            var staleParticipants = _dbSet
                .Where(p => p.SocialEncounterId == encounterId && p.LastSeenAt < cutoff)
                .ToList();

            if (staleParticipants.Count > 0)
            {
                _dbSet.RemoveRange(staleParticipants);
                DbContext.SaveChanges();
            }
        }

        public IEnumerable<SocialEncounterParticipant> GetActive(long encounterId, DateTime referenceTime)
        {
            var cutoff = referenceTime - TimeSpan.FromSeconds(20);
            return _dbSet
                .AsNoTracking() 
                .Where(p => p.SocialEncounterId == encounterId && p.LastSeenAt >= cutoff)
                .ToList();
        }

        public int CountActive(long encounterId, DateTime referenceTime)
        {
            var cutoff = referenceTime - TimeSpan.FromSeconds(20);
            return _dbSet
                .Count(p => p.SocialEncounterId == encounterId && p.LastSeenAt >= cutoff);
        }


    }
}
