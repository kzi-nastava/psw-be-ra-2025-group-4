using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface ITourPointSecretService
{
    TourPointSecretDto GetSecret(int tourPointId, long touristId);
}

