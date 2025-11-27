using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Administration
{
    public interface IFacilityService
    {
        PagedResult<FacilityDto> GetPaged(int page, int pageSize);
        FacilityDto Create(FacilityDto entity);
        FacilityDto Update(FacilityDto entity);
        void Delete(long id);
    }
}
