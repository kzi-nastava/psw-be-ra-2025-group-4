using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TouristEquipmentDbRepository : ITouristEquipmentRepository
{
    protected readonly ToursContext DbContext;
    private readonly DbSet<TouristEquipment> _dbSet;

    public TouristEquipmentDbRepository(ToursContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<TouristEquipment>();
    }

    public List<TouristEquipment> GetForTourist(long touristId)
    {
        return _dbSet.Where(e => e.TouristId == touristId).ToList();
    }

    public bool Exists(long touristId, long equipmentId)
    {
        return _dbSet.Any(e => e.TouristId == touristId && e.EquipmentId == equipmentId);
    }

    public TouristEquipment Create(TouristEquipment entity)
    {
        _dbSet.Add(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public void Delete(long id)
    {
        var entity = _dbSet.Find(id);
        if (entity == null) throw new NotFoundException("Not found: " + id);

        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }
}
