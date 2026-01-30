using System.Linq;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class GuideAssignmentDbRepository : IGuideAssignmentRepository
{
    private readonly ToursContext _db;

    public GuideAssignmentDbRepository(ToursContext db) => _db = db;

    public GuideAssignment? GetActiveForExecution(long executionId)
        => _db.GuideAssignments.FirstOrDefault(a =>
            a.TourExecutionId == executionId && a.Status == GuideAssignmentStatus.Active);

    public bool IsGuideBusy(long guideId)
    {
        return (from a in _db.GuideAssignments
                join te in _db.TourExecutions on a.TourExecutionId equals te.Id
                where a.GuideId == guideId
                      && a.Status == GuideAssignmentStatus.Active
                      && te.Status == TourExecutionStatus.Active
                select a.Id).Any();
    }

    public GuideAssignment Create(GuideAssignment assignment)
    {
        _db.GuideAssignments.Add(assignment);
        _db.SaveChanges();
        return assignment;
    }

    public GuideAssignment Update(GuideAssignment assignment)
    {
        _db.Update(assignment);
        _db.SaveChanges();
        return assignment;
    }
}
