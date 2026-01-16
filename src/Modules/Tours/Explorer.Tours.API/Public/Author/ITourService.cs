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

        void Publish(int tourId, int authorId);
        void Archive(int tourId, int authorId);
        void SetPrice(int tourId, int authorId, decimal price);
        void AddEquipment(int tourId, int authorId, List<EquipmentDto> equipment);
        void AddTourPoint(int tourId, int authorId, TourPointDto tourPoint);
        PagedResult<TourDto> GetPublishedAndArchived(int page, int pageSize);
        TourDto UpdateRouteLength(int tourId, int authorId, double lengthInKm);
        PagedResult<TourDto> GetPublished(int page, int pageSize);
        PagedResult<TourDto> GetPublishedFiltered(int page, int pageSize, string? search, int? difficulty, decimal? minPrice, decimal? maxPrice, List<string>? tags, string? sort, bool? onSale = null);
        IEnumerable<string> GetAllTags();

        PagedResult<PopularTourDto> GetPopular(int authorId, int page, int pageSize, double? lat, double? lon, double? radiusKm);


        PagedResult<AuthorTourDashboardItemDto> GetDashboard(int authorId, int page, int pageSize);

        AuthorTourDashboardDetailsDto GetDashboardDetails(int authorId, int tourId, int days = 30);




    }
}
