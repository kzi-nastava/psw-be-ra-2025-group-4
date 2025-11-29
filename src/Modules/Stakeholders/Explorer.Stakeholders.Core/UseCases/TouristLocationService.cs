using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.UseCases;
using System;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class TouristLocationService : ITouristLocationService
    {
        private readonly ITouristLocationRepository _repository;
        private readonly IMapper _mapper;

        public TouristLocationService(ITouristLocationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public TouristLocationDto SaveOrUpdateLocation(long userId, TouristLocationDto dto)
        {
            var existing = _repository.GetById(userId);

            if (existing == null)
            {
                var newLocation = new TouristLocation(userId, dto.Latitude, dto.Longitude);
                var created = _repository.Save(newLocation);
                return _mapper.Map<TouristLocationDto>(created);
            }

            existing.UpdateLocation(dto.Latitude, dto.Longitude);
            var updated = _repository.Save(existing);
            return _mapper.Map<TouristLocationDto>(updated);
        }

        public TouristLocationDto Get(long userId)
        {
            var location = _repository.GetById(userId);

            return _mapper.Map<TouristLocationDto>(location);
        }
    }
}
