using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class FavoriteTourService : IFavoriteTourService
    {
        private readonly IFavoriteTourRepository _favoriteTourRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public FavoriteTourService(
            IFavoriteTourRepository favoriteTourRepository,
            ITourRepository tourRepository,
            IMapper mapper)
        {
            _favoriteTourRepository = favoriteTourRepository;
            _tourRepository = tourRepository;
            _mapper = mapper;
        }

        public FavoriteTourDto AddFavorite(int touristId, int tourId)
        {
            if (_favoriteTourRepository.Exists(touristId, tourId))
            {
                var existing = _favoriteTourRepository.GetByTouristAndTour(touristId, tourId);
                return _mapper.Map<FavoriteTourDto>(existing);
            }

            var tour = _tourRepository.GetById(tourId);
            if (tour == null)
                throw new ArgumentException("Tour not found");

            var favoriteTour = new FavoriteTour(tourId, touristId, DateTime.UtcNow);
            var created = _favoriteTourRepository.Create(favoriteTour);
            return _mapper.Map<FavoriteTourDto>(created);
        }

        public void RemoveFavorite(int touristId, int tourId)
        {
            var favoriteTour = _favoriteTourRepository.GetByTouristAndTour(touristId, tourId);
            if (favoriteTour == null)
                throw new ArgumentException("Favorite tour not found");

            _favoriteTourRepository.Delete(favoriteTour.Id);
        }

        public bool IsFavorite(int touristId, int tourId)
        {
            return _favoriteTourRepository.Exists(touristId, tourId);
        }

        public PagedResult<TourDto> GetFavoriteTours(int touristId, int page, int pageSize)
        {
            var favorites = _favoriteTourRepository.GetByTourist(touristId).ToList();
            var tourIds = favorites.Select(f => f.TourId).ToList();

            if (!tourIds.Any())
                return new PagedResult<TourDto>(new List<TourDto>(), 0);

            var tours = tourIds.Select(id =>
            {
                try
                {
                    return _tourRepository.GetById(id);
                }
                catch
                {
                    return null;
                }
            })
            .Where(t => t != null)
            .ToList();

            var tourDtos = tours.Select(_mapper.Map<TourDto>).ToList();
            var totalCount = tourDtos.Count;
            var items = tourDtos.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResult<TourDto>(items, totalCount);
        }
    }
}
