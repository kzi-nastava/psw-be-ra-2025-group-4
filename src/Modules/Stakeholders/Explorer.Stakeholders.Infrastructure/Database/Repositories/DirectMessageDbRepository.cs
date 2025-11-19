using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class DirectMessageDbRepository : IDirectMessageRepository
    {
        protected readonly StakeholdersContext DbContext;
        private readonly DbSet<DirectMessage> _dbSet;

        public DirectMessageDbRepository(StakeholdersContext context) 
        {
            DbContext = context;
            _dbSet = DbContext.Set<DirectMessage>();
        }
        public DirectMessage Create(DirectMessage entity)
        {
            _dbSet.Add(entity);
            DbContext.SaveChanges();
            return entity;
        }

        public DirectMessage Get(long id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null) throw new NotFoundException("Not found: " + id);
            return entity;
        }

        public void Delete(long id)
        {
            var entity = Get(id);
            _dbSet.Remove(entity);
            DbContext.SaveChanges();
        }

        public PagedResult<DirectMessage> GetPaged(int page, int pageSize)
        {
            var task = _dbSet.GetPagedById(page, pageSize);
            task.Wait();
            return task.Result;
        }

        public DirectMessage Update(DirectMessage entity)
        {
            DbContext.SaveChanges();
            return entity;
        }
    }
}
