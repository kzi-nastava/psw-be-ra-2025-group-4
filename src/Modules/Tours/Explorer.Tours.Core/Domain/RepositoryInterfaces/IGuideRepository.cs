using System.Collections.Generic;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IGuideRepository
{
    Guide GetById(long id);
    IEnumerable<Guide> GetAll();
    IEnumerable<Guide> GetGuidesForTour(int tourId);
}
