using System.Collections.Generic;
using System.Linq;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourExecutionDbRepository : ITourExecutionRepository
{
    protected readonly ToursContext DbContext;
    private readonly DbSet<TourExecution> _dbSet;

    public TourExecutionDbRepository(ToursContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<TourExecution>();
    }

    public TourExecution Create(TourExecution tourExecution)
    {
        _dbSet.Add(tourExecution);
        DbContext.SaveChanges();
        return tourExecution;
    }

    public TourExecution GetById(long id)
    {
        var entity = _dbSet
            .Include(te => te.CompletedPoints)
            .FirstOrDefault(te => te.Id == id);

        if (entity == null)
            throw new NotFoundException("TourExecution not found: " + id);
        return entity;
    }

    public TourExecution Update(TourExecution tourExecution)
    {
        try
        {
            DbContext.Update(tourExecution);
            DbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
        return tourExecution;
    }

    public IEnumerable<TourExecution> GetByTourist(long touristId)
    {
        return _dbSet
            .Include(te => te.CompletedPoints)
            .Where(te => te.TouristId == touristId)
            .OrderByDescending(te => te.StartTime)
            .ToList();
    }

    public IEnumerable<TourExecution> GetByTouristAndTour(int touristId, int tourId)
    {
        return _dbSet
            .Where(te => te.TouristId == touristId && te.TourId == tourId)
            .Include(te => te.CompletedPoints)
            .OrderByDescending(te => te.LastActivity)
            .ToList();
    }


    public Dictionary<int, ExecutionStats> GetStatsForTours(IEnumerable<int> tourIds)
    {
        var ids = (tourIds ?? Enumerable.Empty<int>()).Distinct().ToList();
        if (!ids.Any()) return new Dictionary<int, ExecutionStats>();

        var data = _dbSet
            .AsNoTracking()
            .Where(x => ids.Contains(x.TourId))
            .GroupBy(x => x.TourId)
            .Select(g => new
            {
                TourId = g.Key,
                Starts = g.Count(),
                Completed = g.Count(x => x.Status == TourExecutionStatus.Completed),
                Abandoned = g.Count(x => x.Status == TourExecutionStatus.Abandoned),
                Active = g.Count(x => x.Status == TourExecutionStatus.Active)
            })
            .ToList();

        return data.ToDictionary(
            x => x.TourId,
            x => new ExecutionStats
            {
                Starts = x.Starts,
                Completed = x.Completed,
                Abandoned = x.Abandoned,
                Active = x.Active
            });
    }

    public List<(DateTime Date, int Count)> GetDailyStarts(int tourId, DateTime from, DateTime to)
    {
        return _dbSet.AsNoTracking()
            .Where(x => x.TourId == tourId && x.StartTime >= from && x.StartTime <= to)
            .GroupBy(x => x.StartTime.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToList()
            .Select(x => (x.Date, x.Count))
            .ToList();
    }

    public List<(DateTime Date, int Count)> GetDailyCompleted(int tourId, DateTime from, DateTime to)
    {
        return _dbSet.AsNoTracking()
            .Where(x => x.TourId == tourId
                && x.Status == TourExecutionStatus.Completed
                && x.EndTime.HasValue
                && x.EndTime.Value >= from && x.EndTime.Value <= to)
            .GroupBy(x => x.EndTime!.Value.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToList()
            .Select(x => (x.Date, x.Count))
            .ToList();
    }

    public List<(DateTime Date, int Count)> GetDailyAbandoned(int tourId, DateTime from, DateTime to)
    {
        return _dbSet.AsNoTracking()
            .Where(x => x.TourId == tourId
                && x.Status == TourExecutionStatus.Abandoned
                && x.EndTime.HasValue
                && x.EndTime.Value >= from && x.EndTime.Value <= to)
            .GroupBy(x => x.EndTime!.Value.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToList()
            .Select(x => (x.Date, x.Count))
            .ToList();
    }



}

