using Explorer.Tours.Core.Domain;

public interface ITourPointRepository
{
    IEnumerable<TourPoint> GetByTour(int tourId);
    TourPoint Get(int id);
    TourPoint Create(TourPoint point);
    TourPoint Update(TourPoint point);
    void Delete(int id);
}
