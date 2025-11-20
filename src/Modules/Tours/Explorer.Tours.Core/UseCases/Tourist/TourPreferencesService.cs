using System;
using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class TourPreferencesService : ITourPreferencesService
    {
        private readonly ITourPreferencesRepository _repository;
        private readonly IMapper _mapper;

        public TourPreferencesService(ITourPreferencesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public TourPreferencesDTO? GetForTourist(int touristId)
        {
            var entity = _repository.GetByTouristId(touristId);
            return entity == null ? null : _mapper.Map<TourPreferencesDTO>(entity);
        }

        public TourPreferencesDTO CreateOrUpdate(int touristId, TourPreferencesDTO dto)
        {
            var existing = _repository.GetByTouristId(touristId);
            var difficulty = Enum.Parse<TourDifficulty>(dto.PreferredDifficulty);

            if (existing == null)
            {
                var entity = new TourPreferences(
                    touristId,
                    difficulty,
                    dto.WalkRating,
                    dto.BikeRating,
                    dto.CarRating,
                    dto.BoatRating,
                    dto.Tags);

                var created = _repository.Create(entity);
                return _mapper.Map<TourPreferencesDTO>(created);
            }

            existing.Update(
                difficulty,
                dto.WalkRating,
                dto.BikeRating,
                dto.CarRating,
                dto.BoatRating,
                dto.Tags);

            var updated = _repository.Update(existing);
            return _mapper.Map<TourPreferencesDTO>(updated);
        }

        public void Delete(int touristId)
        {
            _repository.DeleteByTouristId(touristId);
        }
    }
}
