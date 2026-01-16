using System.Collections.Generic;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourReviewRepository
    {
        TourReview Create(TourReview tourReview);
        TourReview Update(TourReview tourReview);
        void Delete(int id);
        TourReview GetById(int id);
        IEnumerable<TourReview> GetByTourist(int touristId);
        IEnumerable<TourReview> GetByTour(int tourId);
        TourReview GetByTouristAndTour(int touristId, int tourId);

        Dictionary<int, ReviewStats> GetStatsForTours(IEnumerable<int> tourIds);

       
        List<TourReview> GetLatestForTour(int tourId, int take);

      
        List<(DateTime Date, double Avg, int Count)> GetDailyReviewStats(int tourId, DateTime from, DateTime to);
    }
}