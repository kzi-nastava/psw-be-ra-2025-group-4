using System.Collections.Generic;
using System.Linq;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class GuideDbRepository : IGuideRepository
{
    private readonly ToursContext _db;
    private readonly DbSet<Guide> _set;

    public GuideDbRepository(ToursContext db)
    {
        _db = db;
        _set = _db.Set<Guide>();
    }

    public Guide GetById(long id)
    {
        var entity = _set.Include(g => g.Tours).FirstOrDefault(x => x.Id == id);
        if (entity == null) throw new NotFoundException("Guide not found: " + id);
        return entity;
    }

    public IEnumerable<Guide> GetAll()
        => _set.AsNoTracking().Include(g => g.Tours).ToList();

    public IEnumerable<Guide> GetGuidesForTour(int tourId)
        => _set.AsNoTracking()
            .Include(g => g.Tours)
            .Where(g => g.Tours.Any(gt => gt.TourId == tourId))
            .ToList();
}
