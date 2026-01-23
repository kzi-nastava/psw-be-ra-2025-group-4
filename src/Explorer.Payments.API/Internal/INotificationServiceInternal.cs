namespace Explorer.Payments.API.Internal
{
    public interface INotificationServiceInternal
    {
        void CreateMessageNotification(int userId, int actorId, string actorUsername, string content, string? resourceUrl);
    }
}
