using System.Collections.Generic;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourExecutionRepository
{
    TourExecution Create(TourExecution tourExecution);
    TourExecution GetById(long id);
    TourExecution Update(TourExecution tourExecution);
    IEnumerable<TourExecution> GetByTourist(long touristId);

    IEnumerable<TourExecution> GetByTouristAndTour(int touristId, int tourId);
}

