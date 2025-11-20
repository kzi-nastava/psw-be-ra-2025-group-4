using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public PagedResult<DirectMessage> GetPaged(int page, int pageSize, long userId)
        {
            var query = _dbSet
                .Include(dm => dm.Sender)
                .Include(dm => dm.Recipient)
                .Where(message => message.SenderId == userId || message.RecipientId == userId)
                .OrderByDescending(dm => dm.SentAt);

            var task = query.GetPagedById(page, pageSize);
            task.Wait();
            return task.Result;
        }

        public PagedResult<DirectMessage> GetPagedConversations(int page, int pageSize, long userId)
        {
            var userMessages = _dbSet
                .Include(dm => dm.Sender)
                .Include(dm => dm.Recipient)
                .Where(message => message.SenderId == userId || message.RecipientId == userId)
                .OrderByDescending(dm => dm.SentAt)
                .ToList();

            var latestMessages = userMessages
                .GroupBy(message =>
                {
                    var participants = new[] { message.SenderId, message.RecipientId }
                        .OrderBy(x => x)
                        .ToArray();
                    return $"{participants[0]}_{participants[1]}";
                })
                .Select(g => g.First())
                .ToList();

            var totalCount = latestMessages.Count;
            var items = latestMessages
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<DirectMessage>(items, totalCount);
        }


        public DirectMessage Update(DirectMessage entity)
        {
            DbContext.SaveChanges();
            return entity;
        }
    }
}
