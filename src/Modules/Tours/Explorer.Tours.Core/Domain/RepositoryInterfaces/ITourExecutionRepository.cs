using System.Collections.Generic;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourExecutionRepository
{
    TourExecution Create(TourExecution tourExecution);
    TourExecution GetById(long id);
    TourExecution Update(TourExecution tourExecution);
    IEnumerable<TourExecution> GetByTourist(long touristId);

    TourExecution GetActiveTourExecution(long touristId, int tourId);
    bool HasActiveTourExecution(long touristId, int tourId);
    TourExecution GetLastTourExecution(long touristId, int tourId);
}

