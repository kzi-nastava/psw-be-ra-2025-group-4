using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IPersonRepository
{
    Person Create(Person person);

    public PagedResult<Person> GetPaged(int page, int pageSize);
}