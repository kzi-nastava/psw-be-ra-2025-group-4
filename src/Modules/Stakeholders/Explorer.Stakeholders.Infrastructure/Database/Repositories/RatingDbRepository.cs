using System.Collections.Generic;
using System.Linq;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    internal class RatingDbRepository : IRatingRepository
    {
        protected readonly StakeholdersContext DbContext;
        private readonly DbSet<Rating> _dbSet;

        public RatingDbRepository(StakeholdersContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<Rating>();
        }

        public Rating Create(Rating rating)
        {
            _dbSet.Add(rating);
            DbContext.SaveChanges();
            return rating;
        }

        public Rating GetById(long id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null) throw new NotFoundException("Rating not found: " + id);
            return entity;
        }

        public List<Rating> GetAll()
        {
            return _dbSet.AsNoTracking().ToList();
        }

        public List<Rating> GetByUser(long userId)
        {
            return _dbSet.AsNoTracking().Where(r => r.UserId == userId).ToList();
        }

        public Rating Update(Rating rating)
        {
            DbContext.Update(rating);
            DbContext.SaveChanges();
            return rating;
        }

        public void Delete(long id)
        {
            var entity = GetById(id);
            _dbSet.Remove(entity);
            DbContext.SaveChanges();
        }

    }
}
