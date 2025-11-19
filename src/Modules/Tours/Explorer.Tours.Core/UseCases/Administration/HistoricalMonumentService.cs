using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Administration;

public class HistoricalMonumentService : IHistoricalMonumentService
{
    private readonly IHistoricalMonumentRepository _historicalMonumentRepository;
    private readonly IMapper _mapper;

    public HistoricalMonumentService(IHistoricalMonumentRepository repository, IMapper mapper)
    {
        _historicalMonumentRepository = repository;
        _mapper = mapper;
    }

    public PagedResult<HistoricalMonumentDTO> GetPaged(int page, int pageSize)
    {
        var result = _historicalMonumentRepository.GetPaged(page, pageSize);

        var items = result.Results
            .Select(_mapper.Map<HistoricalMonumentDTO>)
            .ToList();

        return new PagedResult<HistoricalMonumentDTO>(items, result.TotalCount);
    }


    public HistoricalMonumentDTO Create(HistoricalMonumentDTO dto)
    {
        var entity = _mapper.Map<HistoricalMonument>(dto);
        var created = _historicalMonumentRepository.Create(entity);
        return _mapper.Map<HistoricalMonumentDTO>(created);
    }

    public HistoricalMonumentDTO Update(HistoricalMonumentDTO dto)
    {
        var entity = _mapper.Map<HistoricalMonument>(dto);
        var updated = _historicalMonumentRepository.Update(entity);
        return _mapper.Map<HistoricalMonumentDTO>(updated);
    }

    public void Delete(long id)
    {
        _historicalMonumentRepository.Delete(id);
    }
}
