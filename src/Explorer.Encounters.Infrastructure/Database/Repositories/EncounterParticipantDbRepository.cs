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
    public class EncounterParticipantDbRepository : IEncounterParticipantRepository
    {
        private readonly EncountersContext _dbContext;
        private readonly DbSet<EncounterParticipant> _dbSet;

        public EncounterParticipantDbRepository(EncountersContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<EncounterParticipant>();
        }

        public EncounterParticipant? Get(long userId)
        {
            return _dbSet.FirstOrDefault(u => u.UserId == userId);
        }

        public EncounterParticipant Add(EncounterParticipant progress)
        {
            _dbSet.Add(progress);
            _dbContext.SaveChanges();
            return progress; ;
        }

        public EncounterParticipant Update(EncounterParticipant progress)
        {
            var i = _dbSet.Update(progress);
            _dbContext.SaveChanges();
            return progress;
        }

        public IEnumerable<EncounterParticipant> GetAll()
        {
            return _dbSet.AsNoTracking().ToList();
        }
    }
}
