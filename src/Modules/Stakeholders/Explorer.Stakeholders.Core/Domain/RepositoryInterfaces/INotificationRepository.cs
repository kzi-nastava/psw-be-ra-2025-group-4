using Explorer.BuildingBlocks.Core.UseCases;
using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface INotificationRepository
    {
        Notification Create(Notification notification);
        Notification Get(long id);
        PagedResult<Notification> GetPaged(long userId, int page, int pageSize);
        void Update(Notification notification);
        void MarkAllAsRead(long userId);
    }
}
