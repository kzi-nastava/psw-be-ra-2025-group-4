using AutoMapper;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Encounters.Core.Mappers;
using Explorer.Encounters.Core.UseCases.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.UseCases;
using Shouldly;
using DomainEncounterType = Explorer.Encounters.Core.Domain.EncounterType;

namespace Explorer.Encounters.Tests.Unit.Tourist;

public class TouristEncounterServiceTests
{
    private readonly IMapper _mapper;

    public TouristEncounterServiceTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<EncountersProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void StartEncounter_returns_dto_when_within_radius()
    {
        // Arrange
        var encounterRepo = new InMemoryEncounterRepository();
        var encounter = new Encounter("Name", "Desc", new Location(19.84, 45.25), 50, DomainEncounterType.Location);
        encounterRepo.Seed(encounter);

        var locationService = new InMemoryTouristLocationService();
        locationService.Seed(encounter.Id + 100, 45.25, 19.84);
        var service = new TouristEncounterService(encounterRepo, locationService, _mapper);

        // Act
        var dto = service.StartEncounter(encounter.Id + 100, encounter.Id);

        // Assert
        dto.ShouldNotBeNull();
        dto.TouristsStarted.ShouldContain(encounter.Id + 100);
    }

    [Fact]
    public void StartEncounter_throws_not_found_when_encounter_missing()
    {
        var encounterRepo = new InMemoryEncounterRepository();
        var locationService = new InMemoryTouristLocationService();
        locationService.Seed(1, 45, 19);
        var service = new TouristEncounterService(encounterRepo, locationService, _mapper);

        Should.Throw<NotFoundException>(() => service.StartEncounter(1, 999));
    }

    [Fact]
    public void StartEncounter_throws_not_found_when_location_missing()
    {
        var encounterRepo = new InMemoryEncounterRepository();
        var encounter = new Encounter("Name", "Desc", new Location(19, 45), 50, DomainEncounterType.Location);
        encounterRepo.Seed(encounter);
        var locationService = new InMemoryTouristLocationService();
        var service = new TouristEncounterService(encounterRepo, locationService, _mapper);

        Should.Throw<NotFoundException>(() => service.StartEncounter(5, encounter.Id));
    }

    [Fact]
    public void StartEncounter_throws_forbidden_when_too_far()
    {
        var encounterRepo = new InMemoryEncounterRepository();
        var encounter = new Encounter("Name", "Desc", new Location(0, 0), 50, DomainEncounterType.Location);
        encounterRepo.Seed(encounter);

        var locationService = new InMemoryTouristLocationService();
        // Roughly 1km away to exceed 500m
        locationService.Seed(10, 0.009, 0);

        var service = new TouristEncounterService(encounterRepo, locationService, _mapper);

        Should.Throw<ForbiddenException>(() => service.StartEncounter(10, encounter.Id));
    }

    private class InMemoryEncounterRepository : IEncounterRepository
    {
        private readonly Dictionary<long, Encounter> _store = new();

        public Encounter Create(Encounter encounter)
        {
            _store[encounter.Id] = encounter;
            return encounter;
        }

        public Encounter? GetById(long id) => _store.TryGetValue(id, out var value) ? value : null;
        public IEnumerable<Encounter> GetActive() => _store.Values;
        public PagedResult<Encounter> GetPaged(int page, int pageSize) => new PagedResult<Encounter>(_store.Values.ToList(), _store.Count);

        public Encounter Update(Encounter encounter)
        {
            _store[encounter.Id] = encounter;
            return encounter;
        }

        public void Delete(long id) => _store.Remove(id);

        public void Seed(Encounter encounter)
        {
            if (encounter.Id == 0)
            {
                // simple auto-id for tests
                var nextId = _store.Keys.DefaultIfEmpty(0).Max() + 1;
                typeof(AggregateRoot).GetProperty("Id")!.SetValue(encounter, nextId);
            }

            _store[encounter.Id] = encounter;
        }
    }

    private class InMemoryTouristLocationService : ITouristLocationService
    {
        private readonly Dictionary<long, TouristLocationDto> _store = new();

        public TouristLocationDto SaveOrUpdateLocation(long userId, TouristLocationDto dto)
        {
            _store[userId] = dto;
            return dto;
        }

        public TouristLocationDto? Get(long userId) => _store.TryGetValue(userId, out var value) ? value : null;

        public void Seed(long userId, double latitude, double longitude)
        {
            _store[userId] = new TouristLocationDto { Latitude = latitude, Longitude = longitude };
        }
    }
}