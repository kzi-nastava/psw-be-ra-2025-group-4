public class UserDto
{
    public string Username { get; private set; }
    public string Role { get; private set; }
    public bool IsActive { get; private set; }

    public UserDto(string username, string role, bool isActive)
    {
        Username = username;
        Role = role;
        IsActive = isActive;
    }
}
