using Explorer.Payments.API.Internal;

namespace Explorer.Payments.Infrastructure.Internal
{
    public class NotificationServiceAdapter : INotificationServiceInternal
    {
        private readonly object _notificationService;

        public NotificationServiceAdapter(object notificationService)
        {
            _notificationService = notificationService;
        }

        public void CreateMessageNotification(int userId, int actorId, string actorUsername, string content, string? resourceUrl)
        {
            var method = _notificationService.GetType().GetMethod("CreateMessageNotification");
            method?.Invoke(_notificationService, new object[] { userId, actorId, actorUsername, content, resourceUrl });
        }
    }
}
