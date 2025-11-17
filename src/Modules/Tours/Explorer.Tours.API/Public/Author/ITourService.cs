using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Author
{
    public interface ITourService
    {
        PagedResult<TourDto> GetPaged(int page, int pageSize);
        TourDto GetById(int id);
        IEnumerable<TourDto> GetByAuthor(int authorId);
        TourDto Create(TourDto tour);
        TourDto Update(TourDto tour);
        void Delete(int id);
    }
}
