using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Encounters.API.Public.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.UseCases;

namespace Explorer.Encounters.Core.UseCases.Tourist
{
    public class TouristEncounterService : ITouristEncounterService
    {
        private const double ActivationRadiusMeters = 500;

        private readonly IEncounterRepository _encounterRepository;
        private readonly ITouristLocationService _touristLocationService;
        private readonly IMapper _mapper;

        public TouristEncounterService(IEncounterRepository encounterRepository, ITouristLocationService touristLocationService, IMapper mapper)
        {
            _encounterRepository = encounterRepository;
            _touristLocationService = touristLocationService;
            _mapper = mapper;
        }   

        public EncounterDto StartEncounter(long touristId, long encounterId)
        {
            var encounter = _encounterRepository.GetById(encounterId);
            if (encounter == null)
                throw new NotFoundException("Encounter not found.");

            var touristLoc = _touristLocationService.Get(touristId);
            if (touristLoc == null)
                throw new NotFoundException("Tourist location not found.");

            var touristLocation = new Location(touristLoc.Longitude, touristLoc.Latitude);
            var distanceMeters = encounter.Location.DistanceToMeters(touristLocation);
            if (distanceMeters > ActivationRadiusMeters)
                throw new ForbiddenException($"Encounter can only be activated within {ActivationRadiusMeters} meters.");

            encounter.StartEncounter(touristId);
            var updatedEncounter = _encounterRepository.Update(encounter);
            return _mapper.Map<EncounterDto>(updatedEncounter);
        }

        public EncounterDto CompleteEncounter(long touristId, long encounterId)
        {
            throw new NotImplementedException();
        }
    }
}