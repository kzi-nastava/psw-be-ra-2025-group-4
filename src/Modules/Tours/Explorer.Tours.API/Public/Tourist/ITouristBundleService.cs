using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist
{
    public interface ITouristBundleService
    {
        PagedResult<BundleDto> GetPublished(int page, int pageSize);
        BundleDto GetById(int id);
    }
}

