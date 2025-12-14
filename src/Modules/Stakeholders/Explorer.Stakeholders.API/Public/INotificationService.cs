using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using System;

namespace Explorer.Stakeholders.API.Public
{
    public interface INotificationService
    {
        PagedResult<NotificationDto> GetPaged(long userId, int page, int pageSize);
        NotificationDto CreateMessageNotification(long userId, long actorId, string actorUsername, string content, string? resourceUrl);
        NotificationDto CreateClubNotification(long userId, string content, long actorId, string actorUsername, long clubId);
        void MarkAsRead(long id);
        void MarkAll(long userId);
        void MarkConversationAsRead(long userId, long actorId);

    }
}
