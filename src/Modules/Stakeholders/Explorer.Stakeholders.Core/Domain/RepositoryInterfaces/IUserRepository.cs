namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IUserRepository
{
    bool Exists(string username);
    User? GetActiveByName(string username);
    User? Get(long userId);
    User Create(User user);
    long GetPersonId(long userId);

    Person? GetPersonByUserId(long userId);
}