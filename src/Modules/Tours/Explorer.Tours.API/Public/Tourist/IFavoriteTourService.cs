using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist
{
    public interface IFavoriteTourService
    {
        FavoriteTourDto AddFavorite(int touristId, int tourId);
        void RemoveFavorite(int touristId, int tourId);
        bool IsFavorite(int touristId, int tourId);
        PagedResult<TourDto> GetFavoriteTours(int touristId, int page, int pageSize);
    }
}
