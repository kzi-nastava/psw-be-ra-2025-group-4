using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.Core.UseCases
{
    public interface ITouristLocationService
    {
        TouristLocationDto SaveOrUpdateLocation(long userId, TouristLocationDto dto);
        TouristLocationDto Get(long userId);
    }
}
