using Explorer.Payments.API.Internal;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases.Internal
{
    public class TourPurchaseTokenService : ITourPurchaseTokenService
    {
        private readonly ITourPurchaseTokenRepository _repository;

        public TourPurchaseTokenService(ITourPurchaseTokenRepository repository)
        {
            _repository = repository;
        }

        public bool HasToken(int touristId, int tourId)
        {
            return _repository.Exists(touristId, tourId);
        }
    }
}