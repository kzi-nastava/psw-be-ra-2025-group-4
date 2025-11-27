using System.Linq;
using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TourProblemService : ITourProblemService
{
    private readonly ITourProblemRepository _repository;
    private readonly IMapper _mapper;

    public TourProblemService(ITourProblemRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public TourProblemDto Create(TourProblemDto tourProblemDto)
    {
        var tourProblem = new TourProblem(
            tourProblemDto.TourId,
            (ProblemCategory)tourProblemDto.Category,
            (ProblemPriority)tourProblemDto.Priority,
            tourProblemDto.Description,
            tourProblemDto.Time,
            tourProblemDto.TouristId
        );

        var created = _repository.Create(tourProblem);
        return _mapper.Map<TourProblemDto>(created);
    }

    public TourProblemDto Update(TourProblemDto tourProblemDto)
    {
        var tourProblem = _repository.GetById(tourProblemDto.Id);

        tourProblem.TourId = tourProblemDto.TourId;
        tourProblem.TouristId = tourProblemDto.TouristId;
        tourProblem.Category = (ProblemCategory)tourProblemDto.Category;
        tourProblem.Priority = (ProblemPriority)tourProblemDto.Priority;
        tourProblem.Description = tourProblemDto.Description;
        tourProblem.Time = tourProblemDto.Time;

        var updated = _repository.Update(tourProblem);
        return _mapper.Map<TourProblemDto>(updated);
    }

    public void Delete(int id)
    {
        _repository.Delete(id);
    }

    public TourProblemDto GetById(int id)
    {
        var tourProblem = _repository.GetById(id);
        return _mapper.Map<TourProblemDto>(tourProblem);
    }

    public PagedResult<TourProblemDto> GetPagedByTourist(int touristId, int page, int pageSize)
    {
        var all = _repository.GetByTourist(touristId).ToList();
        var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        var mapped = items.Select(_mapper.Map<TourProblemDto>).ToList();
        return new PagedResult<TourProblemDto>(mapped, all.Count);
    }
}