using System.Collections.Generic;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourProblemRepository
{
    TourProblem Create(TourProblem tourProblem);
    TourProblem Update(TourProblem tourProblem);
    void Delete(int id);
    TourProblem GetById(int id);
    IEnumerable<TourProblem> GetByTourist(int touristId);
}