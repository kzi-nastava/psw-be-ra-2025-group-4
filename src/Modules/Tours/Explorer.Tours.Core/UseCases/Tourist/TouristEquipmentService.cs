using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TouristEquipmentService : ITouristEquipmentService
{
    private readonly ITouristEquipmentRepository _touristEquipmentRepository;
    private readonly IMapper _mapper;

    public TouristEquipmentService(ITouristEquipmentRepository touristEquipmentRepository, IMapper mapper)
    {
        _touristEquipmentRepository = touristEquipmentRepository;
        _mapper = mapper;
    }

    public List<TouristEquipmentDTO> GetForTourist(long touristId)
    {
        var entities = _touristEquipmentRepository.GetForTourist(touristId);
        return entities.Select(_mapper.Map<TouristEquipmentDTO>).ToList();
    }

    public TouristEquipmentDTO Add(TouristEquipmentDTO dto)
    {
        if (_touristEquipmentRepository.Exists(dto.TouristId, dto.EquipmentId))
        {
            throw new EntityValidationException("Equipment already added for this tourist.");
        }

        var entity = _mapper.Map<TouristEquipment>(dto);
        var created = _touristEquipmentRepository.Create(entity);
        return _mapper.Map<TouristEquipmentDTO>(created);
    }

    public void Remove(long id)
    {
        _touristEquipmentRepository.Delete(id);
    }
}
