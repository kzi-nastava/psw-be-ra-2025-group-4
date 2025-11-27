using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public
{
    public interface ITourPointService
    {
        List<TourPointDto> GetByTour(int tourId, int authorId);
        TourPointDto Create(int tourId, TourPointDto dto, int authorId);
        TourPointDto Update(int pointId, TourPointDto dto, int authorId);
        void Delete(int pointId, int authorId);
    }
}
