using System.Collections.Generic;
using System.Linq;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class ClubMessageDbRepository : IClubMessageRepository
    {
        private readonly StakeholdersContext _dbContext;
        private readonly DbSet<ClubMessage> _dbSet;

        public ClubMessageDbRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<ClubMessage>();
        }

        public ClubMessage Create(ClubMessage message)
        {
            _dbSet.Add(message);
            _dbContext.SaveChanges();
            return message;
        }

        public ClubMessage Update(ClubMessage message)
        {
            _dbContext.Update(message);
            _dbContext.SaveChanges();
            return message;
        }

        public void Delete(long id)
        {
            var entity = GetById(id);
            _dbSet.Remove(entity);
            _dbContext.SaveChanges();
        }

        public ClubMessage GetById(long id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null) throw new NotFoundException("Club message not found: " + id);
            return entity;
        }

        public ClubMessage GetByClubAndAuthor(long clubId, long authorId)
        {
            return _dbSet.FirstOrDefault(m => m.ClubId == clubId && m.AuthorId == authorId);
        }

        public IEnumerable<ClubMessage> GetByClub(long clubId)
        {
            return _dbSet
                .AsNoTracking()
                .Where(m => m.ClubId == clubId)
                .OrderByDescending(m => m.CreatedAt)
                .ToList();
        }
    }
}
