using Explorer.Payments.API.Internal;

namespace Explorer.Payments.Infrastructure.Internal
{
    public class UserInfoServiceAdapter : IUserInfoService
    {
        private readonly object _userService;
        private readonly object _userDiscoveryService;
        private readonly object? _personIdResolver;

        public UserInfoServiceAdapter(object userService, object userDiscoveryService, object? personIdResolver = null)
        {
            _userService = userService;
            _userDiscoveryService = userDiscoveryService;
            _personIdResolver = personIdResolver;
        }

        public UserInfo? GetUser(long userId)
        {
            var getByIdMethod = _userService.GetType().GetMethod("GetById");
            var user = getByIdMethod?.Invoke(_userService, new object[] { userId });
            if (user == null) return null;

            var username = user.GetType().GetProperty("Username")?.GetValue(user)?.ToString();
            var isActive = (bool)(user.GetType().GetProperty("IsActive")?.GetValue(user) ?? false);
            var role = user.GetType().GetProperty("Role")?.GetValue(user)?.ToString();

            return new UserInfo
            {
                Id = userId,
                Username = username ?? "",
                IsActive = isActive,
                IsAdministrator = role == "Administrator"
            };
        }

        public UserInfo? GetUserByUsername(string username)
        {
            var getAllMethod = _userDiscoveryService.GetType().GetMethod("GetAll");
            var allUsers = getAllMethod?.Invoke(_userDiscoveryService, new object[] { 0L }) as System.Collections.IEnumerable;
            if (allUsers == null) return null;

            object? foundUser = null;
            foreach (var user in allUsers)
            {
                var userUsername = user.GetType().GetProperty("Username")?.GetValue(user)?.ToString();
                if (userUsername == username)
                {
                    foundUser = user;
                    break;
                }
            }

            if (foundUser == null) return null;

            var userId = (long)(foundUser.GetType().GetProperty("UserId")?.GetValue(foundUser) ?? 0L);
            return GetUser(userId);
        }

        public long? GetPersonIdByUsername(string username)
        {
            var user = GetUserByUsername(username);
            if (user == null || _personIdResolver == null) return null;
            var method = _personIdResolver.GetType().GetMethod("GetPersonId");
            if (method == null) return null;
            var result = method.Invoke(_personIdResolver, new object[] { user.Id });
            return result is long l ? l : null;
        }

        public bool IsAdministrator(long userId)
        {
            var user = GetUser(userId);
            return user != null && user.IsAdministrator;
        }
    }
}
