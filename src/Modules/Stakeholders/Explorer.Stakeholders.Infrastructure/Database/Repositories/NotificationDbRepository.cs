using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Core.Domain;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class NotificationDbRepository : INotificationRepository
    {
        private readonly StakeholdersContext _context;
        private readonly DbSet<Notification> _set;

        public NotificationDbRepository(StakeholdersContext context)
        {
            _context = context;
            _set = context.Set<Notification>();
        }

        public Notification Create(Notification notification)
        {
            _set.Add(notification);
            _context.SaveChanges();
            return notification;
        }

        public Notification Get(long id)
        {
            var entity = _set.Find(id);
            if (entity == null) throw new NotFoundException($"Notification {id} not found.");
            return entity;
        }

        public PagedResult<Notification> GetPaged(long userId, int page, int pageSize)
        {
            var query = _set
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt);

            var task = query.GetPagedById(page, pageSize);
            task.Wait();
            return task.Result;
        }

        public void Update(Notification notification)
        {
            _context.SaveChanges();
        }

        public void MarkAllAsRead(long userId)
        {
            var unread = _set.Where(n => n.UserId == userId && !n.IsRead).ToList();
            foreach (var n in unread) n.MarkAsRead();
            _context.SaveChanges();
        }
    }
}
