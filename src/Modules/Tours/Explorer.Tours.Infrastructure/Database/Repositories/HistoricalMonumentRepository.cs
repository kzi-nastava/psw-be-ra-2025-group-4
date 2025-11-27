using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class HistoricalMonumentRepository : IHistoricalMonumentRepository
{
    private readonly ToursContext _dbContext;
    private readonly DbSet<HistoricalMonument> _dbSet;

    public HistoricalMonumentRepository(ToursContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.HistoricalMonuments;
    }

    public PagedResult<HistoricalMonument> GetPaged(int page, int pageSize)
    {
        var query = _dbSet.AsQueryable();

        var totalCount = query.Count();

        var results = query
            .OrderBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<HistoricalMonument>(results, totalCount);
    }

    public HistoricalMonument Create(HistoricalMonument entity)
    {
        _dbSet.Add(entity);
        _dbContext.SaveChanges();
        return entity;
    }

    public HistoricalMonument Update(HistoricalMonument entity)
    {
        _dbSet.Update(entity);
        _dbContext.SaveChanges();
        return entity;
    }

    public void Delete(long id)
    {
        var entity = _dbSet.FirstOrDefault(x => x.Id == id);
        if (entity == null) return;

        _dbSet.Remove(entity);
        _dbContext.SaveChanges();
    }
}
