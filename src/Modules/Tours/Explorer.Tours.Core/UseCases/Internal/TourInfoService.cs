using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Internal;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System.Linq;

namespace Explorer.Tours.Core.UseCases.Internal
{
    public class TourInfoService : ITourInfoService
    {
        private readonly ITourRepository _tourRepository;
        private readonly ISaleRepository _saleRepository;

        public TourInfoService(ITourRepository tourRepository, ISaleRepository saleRepository)
        {
            _tourRepository = tourRepository;
            _saleRepository = saleRepository;
        }

        public TourInfoDto Get(int tourId)
        {
            var tour = _tourRepository.GetById(tourId);
            if (tour == null)
                throw new NotFoundException($"Tour {tourId} not found.");

            // Check for active sales
            var activeSales = _saleRepository.GetActiveSalesForTour(tourId);
            decimal finalPrice = tour.Price;
            
            if (activeSales.Any())
            {
                // Take the sale with highest discount
                var bestSale = activeSales.OrderByDescending(s => s.DiscountPercent).First();
                finalPrice = bestSale.CalculateDiscountedPrice(tour.Price);
            }

            return new TourInfoDto
            {
                TourId = (int)tour.Id,
                Name = tour.Name,
                Price = finalPrice, 
                AuthorId = tour.AuthorId,   
                Status = MapStatus(tour.Status)
            };

        }

        private static TourLifecycleStatus MapStatus(TourStatus status)
        {
            return status switch
            {
                TourStatus.Published => TourLifecycleStatus.Published,
                TourStatus.Archived => TourLifecycleStatus.Archived,
                _ => TourLifecycleStatus.Draft
            };
        }
    }
}
