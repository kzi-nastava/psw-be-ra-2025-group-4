using System.Security.Claims;
using Explorer.API.Hubs;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Hubs
{
    [Collection("Sequential")]
    public class MessageHubTests : BaseStakeholdersIntegrationTest
    {
        public MessageHubTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void Hub_can_be_resolved_from_di()
        {
            using var scope = Factory.Services.CreateScope();
            var hub = ActivatorUtilities.CreateInstance<MessageHub>(scope.ServiceProvider);
            hub.ShouldNotBeNull();
        }

        [Fact]
        public async Task OnConnected_adds_user_and_group()
        {
            HubContext.ConnectedUsers.Clear();

            var hub = CreateHub(42, out var groups, out _);

            await hub.OnConnectedAsync();

            HubContext.ConnectedUsers.ContainsKey(42).ShouldBeTrue();
            HubContext.ConnectedUsers[42].ShouldContain("conn-1");
            groups.Added.ShouldContain(x => x.ConnectionId == "conn-1" && x.GroupName == "user_42");
        }

        [Fact]
        public async Task OnDisconnected_removes_user_and_group()
        {
            HubContext.ConnectedUsers.Clear();
            HubContext.ConnectedUsers[42] = ["conn-1"];

            var hub = CreateHub(42, out var groups, out _);

            await hub.OnDisconnectedAsync(null);

            HubContext.ConnectedUsers.ContainsKey(42).ShouldBeFalse();
            groups.Removed.ShouldContain(x => x.ConnectionId == "conn-1" && x.GroupName == "user_42");
        }

        [Fact]
        public async Task Hub_methods_send_messages_to_clients()
        {
            HubContext.ConnectedUsers.Clear();
            HubContext.ConnectedUsers[99] = ["c-99"];

            var hub = CreateHub(42, out _, out var clients);

            await hub.SendMessage(99, "hello", "/r");

            clients.GroupProxies["user_99"].Calls.ShouldContain(c => c.Method == "ReceiveMessage");
            clients.GroupProxies["user_99"].Calls.ShouldContain(c => c.Method == "ReceiveNotification");
            clients.CallerProxy.Calls.ShouldContain(c => c.Method == "MessageSent");

            await hub.UpdateMessage(1, "updated", 99);
            clients.GroupProxies["user_99"].Calls.ShouldContain(c => c.Method == "MessageUpdated");
            clients.CallerProxy.Calls.ShouldContain(c => c.Method == "MessageUpdated");

            await hub.DeleteMessage(1, 99);
            clients.GroupProxies["user_99"].Calls.ShouldContain(c => c.Method == "MessageDeleted");
            clients.CallerProxy.Calls.ShouldContain(c => c.Method == "MessageDeleted");

            await hub.IsRecipientOnline(99);
            clients.CallerProxy.Calls.ShouldContain(c => c.Method == "IsOnline");
            clients.GroupProxies["user_99"].Calls.ShouldContain(c => c.Method == "UserOnline");

            await hub.NotifyTyping(99);
            clients.GroupProxies["user_99"].Calls.ShouldContain(c => c.Method == "UserTyping");
        }

        [Fact]
        public void Hub_has_authorize_attribute()
        {
            var attrs = typeof(MessageHub).GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true);
            attrs.Length.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Hub_inherits_from_signalr_hub()
        {
            typeof(MessageHub).IsSubclassOf(typeof(Hub)).ShouldBeTrue();
        }

        [Fact]
        public void Hubcontext_connected_users_is_dictionary()
        {
            HubContext.ConnectedUsers.ShouldNotBeNull();
            HubContext.ConnectedUsers.GetType().Name.ShouldBe("Dictionary`2");
        }

        [Fact]
        public void Hub_exposes_expected_methods()
        {
            var methods = typeof(MessageHub).GetMethods().Select(m => m.Name).ToList();

            methods.ShouldContain("SendMessage");
            methods.ShouldContain("UpdateMessage");
            methods.ShouldContain("DeleteMessage");
            methods.ShouldContain("NotifyTyping");
            methods.ShouldContain("IsRecipientOnline");
            methods.ShouldContain("OnConnectedAsync");
            methods.ShouldContain("OnDisconnectedAsync");
        }

        private static MessageHub CreateHub(long userId, out GroupManagerStub groups, out HubCallerClientsStub clients)
        {
            clients = new HubCallerClientsStub();
            groups = new GroupManagerStub();
            var dmService = new DirectMessageServiceStub();
            var notifService = new NotificationServiceStub();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("id", userId.ToString()) }));
            var context = new HubCallerContextStub("conn-1", user);

            return new MessageHub(dmService, notifService)
            {
                Clients = clients,
                Groups = groups,
                Context = context
            };
        }
    }

}

namespace Explorer.Stakeholders.Tests.Integration.Hubs
{
    internal sealed class DirectMessageServiceStub : IDirectMessageService
    {
        public DirectMessageDto? LastSent { get; private set; }
        public DirectMessageDto? LastUpdated { get; private set; }
        public long? LastDeletedId { get; private set; }

        public PagedResult<DirectMessageDto> GetPaged(int page, int pageSize, long userId) => throw new NotImplementedException();
        public PagedResult<DirectMessageDto> GetPagedConversations(int page, int pageSize, long userId) => throw new NotImplementedException();
        public PagedResult<DirectMessageDto> GetPagedBetweenUsers(int page, int pageSize, long firstUserId, long secondUserId) => throw new NotImplementedException();

        public DirectMessageDto SendMessage(long senderId, DirectMessageDto directMessage)
        {
            directMessage.Id = 1;
            directMessage.SenderId = senderId;
            directMessage.Sender = "sender";
            LastSent = directMessage;
            return directMessage;
        }

        public DirectMessageDto StartConversation(long senderId, ConversationStartDto directMessage) => throw new NotImplementedException();

        public DirectMessageDto Update(DirectMessageDto directMessage)
        {
            LastUpdated = directMessage;
            return directMessage;
        }

        public void Delete(long id)
        {
            LastDeletedId = id;
        }

        public long EnsureConversation(long firstUserId, string username) => throw new NotImplementedException();
    }

    internal sealed class NotificationServiceStub : INotificationService
    {
        public NotificationDto? LastCreated { get; private set; }

        public PagedResult<NotificationDto> GetPaged(long userId, int page, int pageSize) => throw new NotImplementedException();

        public NotificationDto CreateMessageNotification(long userId, long actorId, string actorUsername, string content, string? resourceUrl)
        {
            LastCreated = new NotificationDto
            {
                ActorId = actorId,
                ActorUsername = actorUsername,
                Content = content,
                ResourceUrl = resourceUrl
            };
            return LastCreated;
        }

        public NotificationDto CreateClubNotification(long userId, string content, long actorId, string actorUsername, long clubId) => throw new NotImplementedException();
        public NotificationDto CreateClubJoinRequestResponseNotification(long userId, long actorId, long clubId, string clubName, bool accepted) => throw new NotImplementedException();
        public void MarkAsRead(long id) => throw new NotImplementedException();
        public void MarkAll(long userId) => throw new NotImplementedException();
        public void MarkConversationAsRead(long userId, long actorId) => throw new NotImplementedException();
        public NotificationDto CreateFollowNotification(long userId, long actorId, string actorUsername, string resourceUrl) => throw new NotImplementedException();
        public NotificationDto CreateAffiliateCodeAssignedNotification(long partnerUserId, long actorId, string? actorUsername, string content, string? resourceUrl) => throw new NotImplementedException();
    }

    internal sealed class ClientProxyStub : IClientProxy
    {
        public readonly List<(string Method, object?[] Args)> Calls = new();

        public Task SendCoreAsync(string method, object?[] args, CancellationToken cancellationToken = default)
        {
            Calls.Add((method, args));
            return Task.CompletedTask;
        }
    }

    internal sealed class HubCallerClientsStub : IHubCallerClients
    {
        public ClientProxyStub AllProxy { get; } = new();
        public ClientProxyStub CallerProxy { get; } = new();
        public ClientProxyStub OthersProxy { get; } = new();
        public ClientProxyStub OthersInGroupProxy { get; } = new();
        public Dictionary<string, ClientProxyStub> GroupProxies { get; } = new();

        public IClientProxy All => AllProxy;
        public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds) => new ClientProxyStub();
        public IClientProxy Caller => CallerProxy;
        public IClientProxy Others => OthersProxy;
        public IClientProxy OthersInGroup(string groupName) => OthersInGroupProxy;
        public IClientProxy Client(string connectionId) => new ClientProxyStub();
        public IClientProxy Clients(IReadOnlyList<string> connectionIds) => new ClientProxyStub();
        public IClientProxy Group(string groupName)
        {
            if (!GroupProxies.TryGetValue(groupName, out var proxy))
            {
                proxy = new ClientProxyStub();
                GroupProxies[groupName] = proxy;
            }
            return proxy;
        }
        public IClientProxy GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds) => new ClientProxyStub();
        public IClientProxy Groups(IReadOnlyList<string> groupNames) => new ClientProxyStub();
        public IClientProxy User(string userId) => new ClientProxyStub();
        public IClientProxy Users(IReadOnlyList<string> userIds) => new ClientProxyStub();
    }

    internal sealed class GroupManagerStub : IGroupManager
    {
        public readonly List<(string ConnectionId, string GroupName)> Added = new();
        public readonly List<(string ConnectionId, string GroupName)> Removed = new();

        public Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
        {
            Added.Add((connectionId, groupName));
            return Task.CompletedTask;
        }

        public Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
        {
            Removed.Add((connectionId, groupName));
            return Task.CompletedTask;
        }
    }

    internal sealed class HubCallerContextStub : HubCallerContext
    {
        private readonly ClaimsPrincipal _user;
        private readonly string _connectionId;

        public HubCallerContextStub(string connectionId, ClaimsPrincipal user)
        {
            _connectionId = connectionId;
            _user = user;
        }

        public override string ConnectionId => _connectionId;
        public override string? UserIdentifier => _user.FindFirst("id")?.Value;
        public override ClaimsPrincipal User => _user;
        public override IDictionary<object, object?> Items { get; } = new Dictionary<object, object?>();
        public override IFeatureCollection Features => new FeatureCollection();
        public override CancellationToken ConnectionAborted => CancellationToken.None;
        public override void Abort() { }
    }
}
