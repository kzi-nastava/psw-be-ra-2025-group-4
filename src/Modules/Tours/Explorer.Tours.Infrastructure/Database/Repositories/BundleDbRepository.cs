using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class BundleDbRepository : IBundleRepository
{
    protected readonly ToursContext DbContext;
    private readonly DbSet<Bundle> _dbSet;

    public BundleDbRepository(ToursContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<Bundle>();
    }

    public PagedResult<Bundle> GetPaged(int page, int pageSize)
    {
        var task = _dbSet
            .Include(b => b.Tours)
            .GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public Bundle GetById(int id)
    {
        var entity = _dbSet
            .Include(b => b.Tours)
            .FirstOrDefault(b => b.Id == id);

        if (entity == null)
            throw new NotFoundException("Bundle not found: " + id);

        return entity;
    }

    public Bundle Create(Bundle bundle)
    {
        DbContext.Bundles.Add(bundle);
        DbContext.SaveChanges();
        return bundle;
    }

    public Bundle Update(Bundle bundle)
    {
        try
        {
            DbContext.Bundles.Update(bundle);
            DbContext.SaveChanges();
            return bundle;
        }
        catch (DbUpdateException e)
        {
            throw new DbUpdateException(e.Message);
        }
    }

    public void Delete(int id)
    {
        var entity = GetById(id);
        DbContext.Bundles.Remove(entity);
        DbContext.SaveChanges();
    }

    public IEnumerable<Bundle> GetByAuthor(int authorId)
    {
        return _dbSet
            .Include(b => b.Tours)
            .Where(b => b.AuthorId == authorId)
            .ToList();
    }
}

