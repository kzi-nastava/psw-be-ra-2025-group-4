using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Administration;

public interface IHistoricalMonumentService
{
    PagedResult<HistoricalMonumentDTO> GetPaged(int page, int pageSize);
    HistoricalMonumentDTO Create(HistoricalMonumentDTO dto);
    HistoricalMonumentDTO Update(HistoricalMonumentDTO dto);
    void Delete(long id);
}
