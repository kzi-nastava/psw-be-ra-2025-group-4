using System;
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System.Linq;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TourExecutionService : ITourExecutionService
{
    private readonly ITourExecutionRepository _executionRepository;
    private readonly ITourRepository _tourRepository;
    private readonly ITourPointRepository _tourPointRepository;
    private readonly ITourPurchaseTokenRepository _tokenRepository;
    private readonly IMapper _mapper;

    public TourExecutionService(
        ITourExecutionRepository executionRepository,
        ITourRepository tourRepository,
        ITourPointRepository tourPointRepository,
        ITourPurchaseTokenRepository tokenRepository,
        IMapper mapper)
    {
        _executionRepository = executionRepository;
        _tourRepository = tourRepository;
        _tourPointRepository = tourPointRepository;
        _tokenRepository = tokenRepository;
        _mapper = mapper;
    }

    public TourExecutionDto StartTour(TourExecutionCreateDto dto, long touristId)
    {
        var existingExecution = _executionRepository.GetByTourist(touristId)
            .FirstOrDefault(te => te.TourId == dto.TourId && te.Status == TourExecutionStatus.Active);
        if (existingExecution != null)
            throw new InvalidOperationException("Tourist already has an active execution for this tour.");

        var tour = _tourRepository.GetById(dto.TourId);
        if (tour.Status != TourStatus.Published && tour.Status != TourStatus.Archived)
            throw new InvalidOperationException("Only published and archived tours can be started.");

        if (!_tokenRepository.Exists((int)touristId, dto.TourId))
            throw new InvalidOperationException("Tour must be purchased before starting.");

        var execution = new TourExecution(touristId, dto.TourId, dto.StartLatitude, dto.StartLongitude);
        var created = _executionRepository.Create(execution);

        var firstPoint = _tourPointRepository.GetByTour(dto.TourId)
            .OrderBy(p => p.Order)
            .FirstOrDefault();

        var dtoResult = _mapper.Map<TourExecutionDto>(created);
        if (firstPoint != null)
        {
            dtoResult.NextKeyPoint = _mapper.Map<TourPointDto>(firstPoint);
        }

        return dtoResult;
    }
    public TourExecutionDto GetById(long executionId, long touristId)
    {
        var execution = _executionRepository.GetById(executionId);

        if (execution.TouristId != touristId)
            throw new ForbiddenException("Not your tour execution.");

        var nextPoint = _tourPointRepository
            .GetByTour(execution.TourId)
            .OrderBy(p => p.Order)
            .FirstOrDefault(p =>
                !execution.CompletedPoints.Any(c => c.TourPointId == p.Id));

        var dto = _mapper.Map<TourExecutionDto>(execution);

        if (nextPoint != null)
        {
            dto.NextKeyPoint = _mapper.Map<TourPointDto>(nextPoint);
        }

        return dto;
    }


    public TourExecutionDto Complete(long executionId, long touristId)
    {
        var execution = _executionRepository.GetById(executionId);
        
        if (execution.TouristId != touristId)
            throw new ForbiddenException("Not your tour execution.");

        execution.Complete();
        var updated = _executionRepository.Update(execution);
        return _mapper.Map<TourExecutionDto>(updated);
    }

    public TourExecutionDto Abandon(long executionId, long touristId)
    {
        var execution = _executionRepository.GetById(executionId);
        
        if (execution.TouristId != touristId)
            throw new ForbiddenException("Not your tour execution.");

        execution.Abandon();
        var updated = _executionRepository.Update(execution);
        return _mapper.Map<TourExecutionDto>(updated);
    }

    public TourExecutionDto Track(long executionId, long touristId, TourExecutionTrackDto dto)
    {
        var execution = _executionRepository.GetById(executionId);
        if (execution.TouristId != touristId)
            throw new ForbiddenException("Not your tour execution.");

        var nextPoint = _tourPointRepository
            .GetByTour(execution.TourId)
            .OrderBy(p => p.Order)
            .FirstOrDefault(p =>
                !execution.CompletedPoints.Any(c => c.TourPointId == p.Id));

        execution.RegisterActivity();

        if (nextPoint != null && IsNear(dto, nextPoint))
        {
            execution.TryCompletePoint(nextPoint.Id);
        }

        _executionRepository.Update(execution);

        return _mapper.Map<TourExecutionDto>(execution);
    }

    private static bool IsNear(TourExecutionTrackDto dto, TourPoint point)
    {
        const double threshold = 0.0002; 

        return Math.Abs(dto.Latitude - point.Latitude) < threshold
            && Math.Abs(dto.Longitude - point.Longitude) < threshold;
    }



    public TourExecutionDto? GetActiveByTour(int tourId, long touristId)
    {
        var activeExecution = _executionRepository.GetByTourist(touristId)
            .FirstOrDefault(te => te.TourId == tourId && te.Status == TourExecutionStatus.Active);

        if (activeExecution == null)
            return null;

        var firstPoint = _tourPointRepository.GetByTour(tourId)
            .OrderBy(p => p.Order)
            .FirstOrDefault();

        var dto = _mapper.Map<TourExecutionDto>(activeExecution);
        if (firstPoint != null)
        {
            dto.NextKeyPoint = _mapper.Map<TourPointDto>(firstPoint);
        }

        return dto;
    }
}

