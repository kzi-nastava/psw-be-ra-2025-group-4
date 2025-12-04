using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public
{
    public interface ITourService
    {
        PagedResult<TourDto> GetPagedByAuthor(int authorId, int page, int pageSize);
        TourDto GetByIdForAuthor(int authorId, int id);
        TourDto GetById(int id);
        TourDto Create(CreateUpdateTourDto dto, int authorId);
        TourDto Update(int id, CreateUpdateTourDto dto, int authorId);
        void DeleteForAuthor(int authorId, int id);
    }
}
