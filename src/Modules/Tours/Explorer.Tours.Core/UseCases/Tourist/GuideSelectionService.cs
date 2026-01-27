using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class GuideSelectionService : IGuideSelectionService
{
    private readonly ITourExecutionRepository _executionRepo;
    private readonly IGuideRepository _guideRepo;
    private readonly IGuideAssignmentRepository _assignmentRepo;
    private readonly IMapper _mapper;

    public GuideSelectionService(
        ITourExecutionRepository executionRepo,
        IGuideRepository guideRepo,
        IGuideAssignmentRepository assignmentRepo,
        IMapper mapper)
    {
        _executionRepo = executionRepo;
        _guideRepo = guideRepo;
        _assignmentRepo = assignmentRepo;
        _mapper = mapper;
    }

    public IEnumerable<GuideDto> GetAvailableGuides(int tourId, long executionId, long touristId)
    {
        var execution = _executionRepo.GetById(executionId);
        if (execution.TouristId != touristId) throw new ForbiddenException("Not your tour execution.");
        if (execution.Status != TourExecutionStatus.Active) return Enumerable.Empty<GuideDto>();
        if (execution.TourId != tourId) throw new ArgumentException("Execution does not match tourId.");

        var guidesForTour = _guideRepo.GetGuidesForTour(tourId);

        var available = guidesForTour
            .Where(g => !_assignmentRepo.IsGuideBusy(g.Id))
            .ToList();

        return available.Select(g => _mapper.Map<GuideDto>(g));
    }

    public void SelectGuide(long executionId, long touristId, long guideId)
    {
        var execution = _executionRepo.GetById(executionId);
        if (execution.TouristId != touristId) throw new ForbiddenException("Not your tour execution.");
        if (execution.Status != TourExecutionStatus.Active)
            throw new InvalidOperationException("Guide can be selected only for active execution.");

        // vodič mora da može ovu turu
        var guide = _guideRepo.GetById(guideId);
        var canGuide = guide.Tours.Any(x => x.TourId == execution.TourId);
        if (!canGuide) throw new InvalidOperationException("Guide cannot lead this tour.");

        var existing = _assignmentRepo.GetActiveForExecution(executionId);
        if (existing != null)
        {
            if (existing.GuideId == guideId) return; 
            existing.Cancel();
            _assignmentRepo.Update(existing);
        }

        if (_assignmentRepo.IsGuideBusy(guideId))
            throw new InvalidOperationException("Guide is not available.");

        _assignmentRepo.Create(new GuideAssignment(guideId, executionId));
    }

    public void CancelGuide(long executionId, long touristId)
    {
        var execution = _executionRepo.GetById(executionId);
        if (execution.TouristId != touristId) throw new ForbiddenException("Not your tour execution.");

        var existing = _assignmentRepo.GetActiveForExecution(executionId);
        if (existing == null) return;

        existing.Cancel();
        _assignmentRepo.Update(existing);
    }
}
