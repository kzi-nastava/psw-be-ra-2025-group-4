using System.Collections.Generic;
using System.Linq;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class ClubDbRepository : IClubRepository
    {
        protected readonly StakeholdersContext DbContext;
        private readonly DbSet<Club> _dbSet;

        public ClubDbRepository(StakeholdersContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<Club>();
        }

        public Club Create(Club club)
        {
            _dbSet.Add(club);
            DbContext.SaveChanges();
            return club;
        }

        public Club GetById(long id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null) throw new NotFoundException("Club not found: " + id);
            return entity;
        }

        public IEnumerable<Club> GetAll()
        {
            return _dbSet.AsNoTracking().ToList();
        }

        public IEnumerable<Club> GetByOwner(long ownerId)
        {
            return _dbSet
                .AsNoTracking()
                .Where(c => c.OwnerId == ownerId)
                .ToList();
        }

        public Club Update(Club club)
        {
            try
            {
                DbContext.Update(club);
                DbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new NotFoundException(e.Message);
            }
            return club;
        }

        public void Delete(long id)
        {
            var entity = GetById(id);
            _dbSet.Remove(entity);
            DbContext.SaveChanges();
        }
    }
}