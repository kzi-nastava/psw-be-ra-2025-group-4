using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourDbRepository : ITourRepository
{
    protected readonly ToursContext DbContext;
    private readonly DbSet<Tour> _dbSet;

    public TourDbRepository(ToursContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<Tour>();
    }

    public PagedResult<Tour> GetPaged(int page, int pageSize)
    {
        var task = _dbSet
            .Include(t => t.Points)
            .Include(t => t.Equipment)
            .GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public Tour GetById(int id)
    {
        var entity = _dbSet
            .Include(t => t.Points)
            .Include(t => t.Equipment)
            .FirstOrDefault(t => t.Id == id);

        if (entity == null)
            throw new NotFoundException("Tour not found: " + id);

        return entity;
    }

    public Tour Create(Tour tour)
    {
        DbContext.Tours.Add(tour);
        DbContext.SaveChanges();
        return tour;
    }

    public Tour Update(Tour tour)
    {
        try
        {
            DbContext.Update(tour);
            DbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
        return tour;
    }

    public void Delete(int id)
    {
        var entity = GetById(id);
        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }

    public IEnumerable<Tour> GetByAuthor(int authorId)
    {
        return _dbSet
            .Include(t => t.Points)
            .Include(t => t.Equipment)
            .Where(t => t.AuthorId == authorId)
            .ToList();
    }

    public IEnumerable<Tour> GetPublishedAndArchived()
    {
        return _dbSet
            .Include(t => t.Points)
            .Where(t => t.Status == TourStatus.Published || t.Status == TourStatus.Archived)
            .ToList();
    }
}
