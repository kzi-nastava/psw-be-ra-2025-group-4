using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IUserRepository
{
    bool Exists(string username);
    User? GetActiveByName(string username);
    User? Get(long userId);
    User Create(User user);
    long GetPersonId(long userId);
    User Update(User user);
    public PagedResult<User> GetPaged(int page, int pageSize);
    User? GetById(long userId);
    IEnumerable<User> GetAllActiveTourists();
    Person? GetPersonByUserId(long userId);
    IEnumerable<User> SearchByUsername(string username);
    User? GetByUsername(string username);
    IEnumerable<User> GetAllActiveNonAdmins();

}