using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourProblemRepository
    {
        TourProblem Create(TourProblem tourProblem);
        TourProblem Update(TourProblem tourProblem);
        void Delete(int id);
        TourProblem GetById(int id);
        IEnumerable<TourProblem> GetByTourist(int touristId);
    }
}