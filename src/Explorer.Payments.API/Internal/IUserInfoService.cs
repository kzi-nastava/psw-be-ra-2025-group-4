namespace Explorer.Payments.API.Internal
{
    public interface IUserInfoService
    {
        UserInfo? GetUser(long userId);
        UserInfo? GetUserByUsername(string username);
        /// <summary>Returns the person id (tourist id in payments) for the given username, or null if not found.</summary>
        long? GetPersonIdByUsername(string username);
        bool IsAdministrator(long userId);
    }

    public class UserInfo
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdministrator { get; set; }
    }
}
