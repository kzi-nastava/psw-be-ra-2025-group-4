using System.Collections.Generic;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourExecutionRepository
{
    TourExecution Create(TourExecution tourExecution);
    TourExecution GetById(long id);
    TourExecution Update(TourExecution tourExecution);
    IEnumerable<TourExecution> GetByTourist(long touristId);

    IEnumerable<TourExecution> GetByTouristAndTour(int touristId, int tourId);

    Dictionary<int, ExecutionStats> GetStatsForTours(IEnumerable<int> tourIds);

    List<(DateTime Date, int Count)> GetDailyStarts(int tourId, DateTime from, DateTime to);
    List<(DateTime Date, int Count)> GetDailyCompleted(int tourId, DateTime from, DateTime to);
    List<(DateTime Date, int Count)> GetDailyAbandoned(int tourId, DateTime from, DateTime to);
    public int GetCompletedToursCountByTourist(long touristId);
}

