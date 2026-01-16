using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class EncounterExecutionDbRepository : IEncounterExecutionRepository
    {
        private readonly EncountersContext _context;

        public EncounterExecutionDbRepository(EncountersContext context)
        {
            _context = context;
        }

        public EncounterExecution? Get(long touristId, long encounterId)
        {
            return _context.Set<EncounterExecution>()
                .SingleOrDefault(x => x.EncounterId == encounterId && x.TouristId == touristId);
        }

        public bool Exists(long touristId, long encounterId)
        {
            return Get(touristId, encounterId) != null;
        }

        public EncounterExecution Create(EncounterExecution execution)
        {
            _context.Add(execution);
            _context.SaveChanges();
            return execution;
        }

        public EncounterExecution Update(EncounterExecution execution)
        {
            _context.Update(execution);
            _context.SaveChanges();
            return execution;
        }

        public void ResolveEncounterForParticipants(long encounterId, IEnumerable<long> touristIdsInRange)
        {
            var touristIds = touristIdsInRange.ToList();

            var existingExecutions = _context.Set<EncounterExecution>()
                .Where(e => e.EncounterId == encounterId && touristIds.Contains(e.TouristId))
                .ToDictionary(e => e.TouristId, e => e);

            foreach (var touristId in touristIds)
            {
                if (existingExecutions.ContainsKey(touristId))
                {
                    continue;
                }

                var execution = new EncounterExecution(touristId, encounterId);

                execution.Complete();

                _context.Add(execution);
            }
            _context.SaveChanges();
        }
    }
}
