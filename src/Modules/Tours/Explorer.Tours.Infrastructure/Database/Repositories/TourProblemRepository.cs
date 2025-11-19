using System.Collections.Generic;
using System.Linq;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourProblemRepository : ITourProblemRepository
{
    protected readonly ToursContext DbContext;
    private readonly DbSet<TourProblem> _dbSet;

    public TourProblemRepository(ToursContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<TourProblem>();
    }

    public TourProblem Create(TourProblem tourProblem)
    {
        _dbSet.Add(tourProblem);
        DbContext.SaveChanges();
        return tourProblem;
    }

    public TourProblem Update(TourProblem tourProblem)
    {
        try
        {
            DbContext.Update(tourProblem);
            DbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
        return tourProblem;
    }

    public void Delete(int id)
    {
        var entity = GetById(id);
        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }

    public TourProblem GetById(int id)
    {
        var entity = _dbSet.Find(id);
        if (entity == null) throw new NotFoundException("TourProblem not found: " + id);
        return entity;
    }

    public IEnumerable<TourProblem> GetByTourist(int touristId)
    {
        return _dbSet.Where(tp => tp.TouristId == touristId).ToList();
    }
}