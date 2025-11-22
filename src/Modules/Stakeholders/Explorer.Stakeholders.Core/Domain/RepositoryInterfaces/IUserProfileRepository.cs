using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IUserProfileRepository
    {
        UserProfile GetByUserId(long userId);
        UserProfile Update(UserProfile profile);
    }
}
