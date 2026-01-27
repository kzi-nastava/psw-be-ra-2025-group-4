using System.Collections.Generic;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IFavoriteTourRepository
    {
        FavoriteTour Create(FavoriteTour favoriteTour);
        void Delete(int id);
        FavoriteTour GetById(int id);
        FavoriteTour GetByTouristAndTour(int touristId, int tourId);
        IEnumerable<FavoriteTour> GetByTourist(int touristId);
        bool Exists(int touristId, int tourId);
    }
}
