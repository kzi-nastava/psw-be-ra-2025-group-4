using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IHistoricalMonumentRepository
{
    PagedResult<HistoricalMonument> GetPaged(int page, int pageSize);
    HistoricalMonument Create(HistoricalMonument entity);
    HistoricalMonument Update(HistoricalMonument entity);
    void Delete(long id);
}
