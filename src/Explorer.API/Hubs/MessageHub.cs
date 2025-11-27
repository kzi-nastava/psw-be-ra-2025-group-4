using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Npgsql.Replication.PgOutput.Messages;
using NuGet.Protocol.Plugins;
using System.Security.Claims;

namespace Explorer.API.Hubs
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IDirectMessageService _directMessageService;

        public MessageHub(IDirectMessageService directMessageService)
        {
            _directMessageService = directMessageService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();

            if (!HubContext.ConnectedUsers.ContainsKey(userId))
            {
                HubContext.ConnectedUsers[userId] = [Context.ConnectionId];
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();

            if (HubContext.ConnectedUsers.ContainsKey(userId))
            {
                HubContext.ConnectedUsers[userId].Remove(Context.ConnectionId);
                if (HubContext.ConnectedUsers[userId].Count == 0)
                    HubContext.ConnectedUsers.Remove(userId);
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(long recipientId, string content)
        {
            try
            {
                var senderId = GetUserId();

                var messageDto = new DirectMessageDto
                {
                    RecipientId = recipientId,
                    Content = content
                };

                var sentMessage = _directMessageService.SendMessage(senderId, messageDto);

                await Clients.Group($"user_{recipientId}").SendAsync("ReceiveMessage", sentMessage);

                await Clients.Caller.SendAsync("MessageSent", sentMessage);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

        public async Task UpdateMessage(long messageId, string content, long recipientId)
        {
            try
            {
                var messageDto = new DirectMessageDto
                {
                    Id = messageId,
                    Content = content,
                    RecipientId = recipientId
                };

                var updatedMessage = _directMessageService.Update(messageDto);

                await Clients.Group($"user_{recipientId}").SendAsync("MessageUpdated", updatedMessage);

                await Clients.Caller.SendAsync("MessageUpdated", updatedMessage);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

        public async Task IsRecipientOnline(long recipientId)
        {
            try
            {
                var senderId = GetUserId(); 
                bool isOnline = HubContext.ConnectedUsers.ContainsKey(recipientId);

                await Clients.Caller.SendAsync("IsOnline", isOnline);

                await Clients.Group($"user_{recipientId}").SendAsync("UserOnline", senderId);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }


        public async Task DeleteMessage(long messageId, long recipientId)
        {
            try
            {
                _directMessageService.Delete(messageId);

                await Clients.Group($"user_{recipientId}").SendAsync("MessageDeleted", messageId);

                await Clients.Caller.SendAsync("MessageDeleted", messageId);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

        public async Task NotifyTyping(long recipientId)
        {
            var senderId = GetUserId();
            await Clients.Group($"user_{recipientId}").SendAsync("UserTyping", senderId);
        }

        private long GetUserId()
        {
            return Convert.ToInt64(Context.User.FindFirst("id").Value);
        }
    }
}
