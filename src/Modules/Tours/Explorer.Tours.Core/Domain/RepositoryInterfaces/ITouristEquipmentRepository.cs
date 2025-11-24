using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITouristEquipmentRepository
{
    List<TouristEquipment> GetForTourist(long touristId);
    TouristEquipment Create(TouristEquipment entity);
    void Delete(long id);

    bool Exists(long touristId, long equipmentId);
}

