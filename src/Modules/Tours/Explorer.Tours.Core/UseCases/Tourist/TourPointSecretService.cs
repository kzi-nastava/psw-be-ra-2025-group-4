using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TourPointSecretService : ITourPointSecretService
{
    private readonly ITourPointRepository _tourPointRepository;
    private readonly ITourExecutionRepository _tourExecutionRepository;

    public TourPointSecretService(
        ITourPointRepository tourPointRepository,
        ITourExecutionRepository tourExecutionRepository)
    {
        _tourPointRepository = tourPointRepository;
        _tourExecutionRepository = tourExecutionRepository;
    }

    public TourPointSecretDto GetSecret(long tourPointId, long touristId)
    {
        var tourPoint = _tourPointRepository.Get(tourPointId);
        if (tourPoint == null)
            throw new NotFoundException($"Tour point with id {tourPointId} not found.");

        var touristExecutions = _tourExecutionRepository.GetByTourist(touristId);
        var hasCompletedPoint = touristExecutions
            .Any(execution => execution.CompletedPoints
                .Any(cp => cp.TourPointId == tourPointId));

        if (!hasCompletedPoint)
            throw new ForbiddenException("You must complete the point before accessing its secret.");

        return new TourPointSecretDto
        {
            TourPointId = tourPointId,
            Secret = tourPoint.Secret,
            IsUnlocked = true
        };
    }
}


