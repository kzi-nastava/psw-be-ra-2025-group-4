using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.Repositories;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class HiddenLocationEncounterDbRepository : IHiddenLocationEncounterRepository
    {
        private readonly EncountersContext _context;
        private readonly DbSet<HiddenLocationEncounter> _dbSet;

        public HiddenLocationEncounterDbRepository(EncountersContext context)
        {
            _context = context;
            _dbSet = _context.Set<HiddenLocationEncounter>();
        }

        public HiddenLocationEncounter Create(HiddenLocationEncounter encounter)
        {
            _dbSet.Add(encounter);
            _context.SaveChanges();
            return encounter;
        }

        public HiddenLocationEncounter Get(long id)
        {
            var entity = _dbSet.FirstOrDefault(e => e.Id == id);

            if (entity == null)
                throw new NotFoundException("Hidden location encounter not found: " + id);

            return entity;
        }

        public HiddenLocationEncounter Update(HiddenLocationEncounter encounter)
        {
            try
            {
                _context.Update(encounter);
                _context.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new NotFoundException(e.Message);
            }

            return encounter;
        }
    }
}
