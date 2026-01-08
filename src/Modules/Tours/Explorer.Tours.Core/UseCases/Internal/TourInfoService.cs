using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Internal;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Internal
{
    public class TourInfoService : ITourInfoService
    {
        private readonly ITourRepository _tourRepository;

        public TourInfoService(ITourRepository tourRepository)
        {
            _tourRepository = tourRepository;
        }

        public TourInfoDto Get(int tourId)
        {
            var tour = _tourRepository.GetById(tourId);
            if (tour == null)
                throw new NotFoundException($"Tour {tourId} not found.");

            return new TourInfoDto
            {
                TourId = (int)tour.Id,
                Name = tour.Name,
                Price = tour.Price,
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
