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
        var entity = _dbSet.FirstOrDefault(te => te.Id == id);
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
            .Where(te => te.TouristId == touristId)
            .OrderByDescending(te => te.StartTime)
            .ToList();
    }
}

