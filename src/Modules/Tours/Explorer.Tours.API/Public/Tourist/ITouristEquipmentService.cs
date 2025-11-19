using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface ITouristEquipmentService
{
    List<TouristEquipmentDTO> GetForTourist(long touristId);

    TouristEquipmentDTO Add(TouristEquipmentDTO touristEquipment);

    void Remove(long id);
}

