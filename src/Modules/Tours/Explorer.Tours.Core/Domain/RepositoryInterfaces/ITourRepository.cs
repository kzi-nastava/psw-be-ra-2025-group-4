using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourRepository
    {
        PagedResult<Tour> GetPaged(int page, int pageSize);
        Tour GetById(int id);
        Tour Create(Tour tour);
        Tour Update(Tour tour);
        void Delete(int id);
        IEnumerable<Tour> GetByAuthor(int authorId);
        IEnumerable<Tour> GetPublishedAndArchived();
        IEnumerable<Tour> GetPublished();
        IQueryable<Tour> QueryPublished();

    }
}
