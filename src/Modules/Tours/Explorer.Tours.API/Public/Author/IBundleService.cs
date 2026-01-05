using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Author
{
    public interface IBundleService
    {
        PagedResult<BundleDto> GetPagedByAuthor(int authorId, int page, int pageSize);
        BundleDto GetByIdForAuthor(int authorId, int id);
        BundleDto Create(CreateBundleDto dto, int authorId);
        BundleDto Update(int id, UpdateBundleDto dto, int authorId);
        void Delete(int id, int authorId);
        void Publish(int id, int authorId);
        void Archive(int id, int authorId);
    }
}

