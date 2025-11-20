using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public;

public interface ITourProblemService
{
    TourProblemDto Create(TourProblemDto tourProblemDto);
    TourProblemDto Update(TourProblemDto tourProblemDto);
    void Delete(int id);
    TourProblemDto GetById(int id);
    PagedResult<TourProblemDto> GetPagedByTourist(int touristId, int page, int pageSize);
}