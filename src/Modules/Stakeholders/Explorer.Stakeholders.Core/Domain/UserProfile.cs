using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public class UserProfile : Entity
{
    public long UserId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string? Biography { get; private set; }
    public string? Motto { get; private set; }
    public string? ProfileImageUrl { get; private set; }

    public UserProfile(long userId, string firstName, string lastName, string? biography, string? motto, string? profileImageUrl)
    {
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        Biography = biography;
        Motto = motto;
        ProfileImageUrl = profileImageUrl;

        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(FirstName))
            throw new ArgumentException("Invalid FirstName");

        if (string.IsNullOrWhiteSpace(LastName))
            throw new ArgumentException("Invalid LastName");

        
    }

    public void Update(string firstName, string lastName, string? biography, string? motto, string? profileImageUrl)
    {
        FirstName = firstName;
        LastName = lastName;
        Biography = biography;
        Motto = motto;
        ProfileImageUrl = profileImageUrl;

        Validate();
    }
}
