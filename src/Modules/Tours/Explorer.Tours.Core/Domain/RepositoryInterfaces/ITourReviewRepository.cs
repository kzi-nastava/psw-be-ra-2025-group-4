using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourReviewRepository
    {
        TourReview Create(TourReview review);
        TourReview GetById(long id);
        TourReview Update(TourReview review);
        void Delete(long id);
        TourReview GetByTouristAndTour(long touristId, int tourId);
        bool HasReview(long touristId, int tourId);
    }
}