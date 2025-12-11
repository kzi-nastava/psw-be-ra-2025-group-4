using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System.Linq;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;
        private readonly IMapper _mapper;

        public NotificationService(INotificationRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public NotificationDto CreateMessageNotification(long userId, string content, string? resourceUrl)
        {
            var notif = new Notification(userId, content, NotificationType.Message, resourceUrl);
            var created = _repo.Create(notif);
            return _mapper.Map<NotificationDto>(created);
        }

        public NotificationDto CreateClubNotification(long userId, string content, string? resourceUrl)
        {
            var notif = new Notification(userId, content, NotificationType.ClubActivity, resourceUrl);
            var created = _repo.Create(notif);
            return _mapper.Map<NotificationDto>(created);
        }

        public PagedResult<NotificationDto> GetPaged(long userId, int page, int pageSize)
        {
            var result = _repo.GetPaged(userId, page, pageSize);
            var mapped = result.Results.Select(_mapper.Map<NotificationDto>).ToList();
            return new PagedResult<NotificationDto>(mapped, result.TotalCount);
        }

        public void MarkAsRead(long id)
        {
            var notif = _repo.Get(id);
            notif.MarkAsRead();
            _repo.Update(notif);
        }

        public void MarkAll(long userId)
        {
            _repo.MarkAllAsRead(userId);
        }
    }
}
